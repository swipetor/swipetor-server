using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoreLinq;
using SwipetorApp.Areas.Api.Models;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Models.DTOs;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services;
using SwipetorApp.Services.Auth;
using SwipetorApp.Services.Contexts;
using SwipetorApp.Services.Permissions;
using SwipetorApp.Services.Posts;
using SwipetorApp.Services.SqlQueries;
using SwipetorApp.Services.VideoServices;
using SwipetorApp.System;
using WebAppShared.DI;
using WebAppShared.Exceptions;
using WebAppShared.Metrics;

namespace SwipetorApp.Areas.Api;

[ApiController]
[Area(AreaNames.Api)]
[Route("api/posts")]
public class PostsApi(
    UserCx userCx,
    IDbProvider dbProvider,
    IMapper mapper,
    PostSvc postSvc,
    ILogger<PostsApi> logger,
    IMetricsSvc metricsSvc,
    IFactory<VideoMediaUpdaterSvc> videoMediaUpdaterFactory)
    : Controller
{
    [HttpGet("/api/posts")]
    public IActionResult Get([FromQuery] PostsGetRequestModel model)
    {
        var hubsArr = model.HubIds?.Split(',').Select(int.Parse).ToList() ?? [];

        using var querier = new PostsQuerier(userCx.ValueOrNull);
        querier.FirstPostId = model.FirstPostId;
        querier.UserId = model.UserId;
        querier.HubIds = hubsArr;

        using var db = dbProvider.Create();
        var posts = querier.Run(db);

        var res = new
        {
            posts = mapper.Map<List<PostDto>>(posts)
        };

        return Json(res);
    }

    /// <summary>
    ///     Get single post
    /// </summary>
    /// <param name="postId"></param>
    /// <returns></returns>
    [HttpGet("{postId}")]
    public IActionResult GetByPostId(int postId)
    {
        using var db = dbProvider.Create();
        var postsQuery = db.Posts.Where(p => p.Id == postId);
        var post = postsQuery.SelectForUser(userCx.ValueOrNull?.Id).Take(1).ToList().FirstOrDefault();

        if (post == null) return NotFound();

        var postDto = mapper.Map<PostDto>(post);

        return Json(postDto);
    }

    [HttpPut("{postId}")]
    [Authorize]
    [MinRole(UserRole.Creator)]
    public async Task<IActionResult> Update(int postId, PostSaveModel model)
    {
        await using var db = dbProvider.Create();
        var post = db.Posts
            .Include(p => p.PostHubs).ThenInclude(c => c.Hub)
            .Include(p => p.Medias).ThenInclude(postMedia => postMedia.Video)
            .Single(p => p.Id == postId);

        new PostPerms().AssertCanEdit(post, userCx.Value);

        var firstPostId = post.Medias.MinBy(m => m.Ordering)?.Id;
        if (model.Items.SingleOrDefault(i => i.Id == firstPostId)?.IsFollowersOnly == true)
            throw new HttpJsonError("First post item cannot be paid");
        
        if (model.Items.Any(i => i.SubPlanId != null) && model.Items.Any(i => i.IsFollowersOnly))
            throw new HttpJsonError("A post media cannot be both followers only and paid");

        post.PostHubs.ForEach(c => c.Hub.PostCount++);
        post.IsPublished = model.IsPublished;

        if (model.PosterUserId != null) post.UserId = model.PosterUserId.Value;

        foreach (var item in model.Items)
        {
            using var mediaUpdater = videoMediaUpdaterFactory.GetInstance();
            await mediaUpdater.Update(item);
        }

        if (model.HubIds is { Length: > 0 })
            postSvc.UpdateHubs(post.Id, model.HubIds.ToList(),
                post.PostHubs.Select(c => c.HubId).ToList());

        metricsSvc.Collect("PostUpdated", 1);
        metricsSvc.Collect(model.IsPublished ? "PostPublished" : "PostSaved", 1);
        
        await db.SaveChangesAsync();

        // Start notif batch if this is the first time the post is published
        if (model.IsPublished)
        {
            postSvc.StartNotifBatchIfNotExists(post.Id);
        }

        return Ok();
    }

    [HttpDelete("{postId}")]
    [Authorize]
    [MinRole(UserRole.Creator)]
    public IActionResult Delete(int postId)
    {
        using var db = dbProvider.Create();
        var post = db.Posts.Single(t => t.Id == postId);

        new PostPerms().AssertCanDelete(post, userCx.Value);

        post.IsRemoved = true;
        db.SaveChanges();

        metricsSvc.Collect("PostRemoved", 1);

        return Ok();
    }

    [Authorize]
    [HttpGet("my")]
    public IActionResult MyPosts(bool isPublished = true)
    {
        using var db = dbProvider.Create();

        var postsQuery = db.Posts
            .Where(p => p.IsPublished == isPublished && p.UserId == userCx.Value.Id && !p.IsRemoved)
            .OrderByDescending(p => p.ModifiedAt)
            .ThenBy(p => p.ModifiedAt);
        var post = postsQuery.ToList();

        var postDto = mapper.Map<List<PostDto>>(post);
        return Json(postDto);
    }

    /// <summary>
    ///     Favourite posts
    /// </summary>
    /// <returns></returns>
    [HttpGet("favs")]
    [Authorize]
    public IActionResult Favs()
    {
        using var db = dbProvider.Create();
        var cu = userCx.Value;

        var posts = db.Posts
            .Where(p => p.Favs.Any(fp => fp.UserId == cu.Id))
            .OrderByDescending(p => p.Favs.FirstOrDefault(fp => fp.UserId == cu.Id).CreatedAt)
            .SelectForUser(cu.Id)
            .ToList();

        var dtos = mapper.Map<List<PostDto>>(posts);

        return Json(dtos);
    }

    [HttpPost("{postId:int}/fav")]
    [Authorize]
    public IActionResult Save(int postId)
    {
        using var db = dbProvider.Create();
        var cu = userCx.Value;

        var alreadySaved = db.FavPosts.Any(sp => sp.UserId == cu.Id && sp.PostId == postId);

        if (alreadySaved) return Ok();

        db.FavPosts.Add(new FavPost
        {
            UserId = cu.Id,
            PostId = postId
        });

        db.SaveChanges();

        return Ok();
    }

    [HttpDelete("{postId:int}/fav")]
    [Authorize]
    public IActionResult Unsave(int postId)
    {
        using var db = dbProvider.Create();
        var cu = userCx.Value;

        db.FavPosts.Where(sp => sp.UserId == cu.Id && sp.PostId == postId).DeleteFromQuery();

        return Ok();
    }

    [HttpGet("my-drafts")]
    [Authorize]
    public IActionResult MyDrafts()
    {
        if (!userCx.IsLoggedIn) return Json("[]");
        using var db = dbProvider.Create();

        var postsQuery = db.Posts.Where(p =>
                !p.IsPublished && p.UserId == userCx.Value.Id && !p.IsRemoved)
            .OrderByDescending(p => p.ModifiedAt);
        var post = postsQuery.ToList();

        var postDto = mapper.Map<List<PostDto>>(post);
        return Json(postDto);
    }

    [HttpGet("all")]
    [Authorize]
    public IActionResult All(bool? isPublished)
    {
        using var db = dbProvider.Create();

        var postsQuery = db.Posts.Where(p => !p.IsRemoved && p.UserId == userCx.Value.Id);

        if (isPublished != null)
            postsQuery = postsQuery.Where(p => p.IsPublished == isPublished);

        var posts = postsQuery.OrderByDescending(p => p.ModifiedAt)
            .ThenBy(p => p.ModifiedAt)
            .ToList();

        var postDto = mapper.Map<List<PostDto>>(posts);
        return Json(postDto);
    }


    /// <summary>
    ///     Create a new Draft post
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("my-drafts")]
    [Authorize]
    [MinRole(UserRole.Creator)]
    public IActionResult CreateDraft(CreateDraftPostViewModel model)
    {
        var post = new Post
        {
            Title = model.Title,
            UserId = userCx.Value.Id,
            IsPublished = false,
            ModifiedAt = DateTime.UtcNow
        };

        using (var db = dbProvider.Create())
        {
            db.Posts.Add(post);
            db.SaveChanges();
        }

        metricsSvc.Collect("DraftPostCreated", 1);

        return Json(mapper.Map<PostDto>(post));
    }

    [HttpPut("{postId}/title")]
    [Authorize]
    [MinRole(UserRole.Creator)]
    public IActionResult Title(int postId, PostsApiTitleUpdateReqModel model)
    {
        using var db = dbProvider.Create();
        var post = db.Posts.Single(t => t.Id == postId);

        new PostPerms().AssertCanEdit(post, userCx.Value);

        post.Title = model.Title;
        db.SaveChanges();
        return Ok();
    }


    [HttpDelete("{postId}/publish")]
    [Authorize]
    [MinRole(UserRole.Creator)]
    public IActionResult Unpublish(int postId)
    {
        using var db = dbProvider.Create();
        var post = db.Posts
            .Include(p => p.PostHubs).ThenInclude(c => c.Hub)
            .Single(p => p.Id == postId);

        new PostPerms().AssertCanEdit(post, userCx.Value);

        post.PostHubs.ForEach(c => c.Hub.PostCount--);
        post.IsPublished = false;

        db.SaveChanges();

        return Ok();
    }
}