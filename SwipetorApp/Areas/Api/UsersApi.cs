using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SwipetorApp.Areas.Api.Models;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Models.DTOs;
using SwipetorApp.Models.EmailViewModels;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Auth;
using SwipetorApp.Services.Config;
using SwipetorApp.Services.Contexts;
using SwipetorApp.Services.Emailing;
using SwipetorApp.Services.Pm;
using SwipetorApp.Services.SqlQueries;
using SwipetorApp.Services.Users;
using SwipetorApp.System;
using WebAppShared.Exceptions;
using WebAppShared.Extensions;
using WebAppShared.WebSys.MvcAttributes;

namespace SwipetorApp.Areas.Api;

[ApiController]
[Area(AreaNames.Api)]
[Route("api/users")]
public class UsersApi(IMapper mapper, IDbProvider dbProvider, PmThreadSvc pmThreadSvc, IOptions<SiteConfig> siteConfig, EmailerProvider emailerProvider, UserCx userCx)
    : Controller
{
    [HttpGet("{userId:int}")]
    public IActionResult GetById(int userId, bool includePosts = false)
    {
        var myId = userCx.ValueOrNull?.Id;
        using var db = dbProvider.Create();
        var result = db.Users.Where(u => u.Id == userId).Include(u => u.Photo)
            .Select(u => new 
            {
                user = u,
                userFollows = db.UserFollows.Any(uf => uf.FollowerUserId == myId && uf.FollowedUserId == u.Id)
            })
            .SingleOrDefault();

        if (result == null) return NotFound();
        
        var resp = new UsersApiGetUsersResp
        {
            User = mapper.Map<PublicUserDto>(result.user)
        };

        if (resp.User != null)
        {
            resp.User.UserFollows = result.userFollows;
            
            if (includePosts)
            {
                var posts = db.Posts.Where(p => p.UserId == userId && p.IsPublished && !p.IsRemoved)
                    .OrderByDescending(p => p.CreatedAt)
                    .SelectForUser(myId).ToList();
                resp.Posts = mapper.Map<List<PostDto>>(posts);
            }
            
            resp.PmThreadId = pmThreadSvc.GetThreadIfExists([resp.User.Id])?.Id;
            resp.CanMsg = resp.PmThreadId != null || result.userFollows || pmThreadSvc.HasCommentedMeLastWeek(resp.User.Id);
        }

        return Json(resp);
    }

    [HttpPost("{followedUserId:int}/follow")]
    [Authorize]
    public IActionResult Follow(int followedUserId)
    {
        var myId = userCx.Value.Id;
        using var db = dbProvider.Create();
        var isFollowing =
            db.UserFollows.Any(uf => uf.FollowerUserId == myId && uf.FollowedUserId == followedUserId);

        if (isFollowing) return Ok();

        var userFollow = new UserFollow
        {
            FollowerUserId = myId,
            FollowedUserId = followedUserId
        };

        db.UserFollows.Add(userFollow);
        db.SaveChanges();
        return Ok();
    }

    [HttpDelete("{followedUserId:int}/follow")]
    [Authorize]
    public IActionResult DeleteFollow(int followedUserId)
    {
        var myId = userCx.Value.Id;
        using var db = dbProvider.Create();
        db.UserFollows.Where(uf => uf.FollowerUserId == myId && uf.FollowedUserId == followedUserId)
            .DeleteFromQuery();
        return Ok();
    }

    [Authorize]
    [Route("search")]
    [MinRole(UserRole.Default)]
    public IActionResult Search([Required] [MinLength(3)] string username, UserRole? minRole = null)
    {
        using var db = dbProvider.Create();
        var query = db.Users.Where(u => EF.Functions.ILike(u.Username, $"%{username.Trim()}%"));

        var users = query.Include(u => u.Photo).Take(10).ToList();
        var userDtos = mapper.Map<List<PublicUserDto>>(users);
        return Json(userDtos);
    }
    
    [Authorize]
    [Route("become-creator")]
    public async Task<IActionResult> BecomeCreator(UserApiBecomeCreatorReqModel model)
    {
        var me = userCx.Value;
        if (!ModelState.IsValid) throw new HttpJsonError("Invalid model");
        
        var txt = $"User: {me.Username} #{me.Id}<p>----------</p><p>{model.Txt}</p>";
        
        var genericEmailVM = new GenericEmailVM
        {
            Text = txt
        };
        
        using var emailer = emailerProvider.GetGenericEmailer<GenericEmailVM>();
        await emailer.Send(siteConfig.Value.Email, "Become Creator Request", genericEmailVM);

        return Ok();
    }
}