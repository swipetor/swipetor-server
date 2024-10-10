using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SwipetorApp.Areas.Admin.ViewModels;
using SwipetorApp.Models.DTOs;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Auth;
using SwipetorApp.Services.Contexts;
using SwipetorApp.Services.SqlQueries;
using SwipetorApp.System.UserRoleAuth;

namespace SwipetorApp.Areas.Admin;

[Area(AreaNames.Admin)]
[UserRoleAuth(UserRole.Admin)]
public class PostsController(IMapper mapper, IDbProvider dbProvider) : Controller
{
    public IActionResult Index(bool isRemoved, int? hubId)
    {
        using var db = dbProvider.Create();

        var q = db.Posts.AsQueryable();
        q = q.Where(p => p.IsRemoved == isRemoved);
        if (hubId != null) q = q.Where(p => p.PostHubs.Any(pc => pc.HubId == hubId));

        q = q.Take(1000);

        var posts = q.SelectForUser(null);

        var hubs = db.Hubs.SelectForUser().ToList();

        var postsDto = mapper.Map<List<PostDto>>(posts);
        var hubsDto = mapper.Map<List<HubDto>>(hubs);

        return View(new PostsIndexViewModel
        {
            Posts = postsDto,
            Hubs = hubsDto,
            IsRemoved = isRemoved,
            HubId = hubId
        });
    }
}