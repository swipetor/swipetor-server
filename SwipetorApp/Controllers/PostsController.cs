using System;
using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwipetorApp.Models.Enums;
using SwipetorApp.Models.Extensions;
using SwipetorApp.Models.ViewModels;
using SwipetorApp.Services.Contexts;
using WebAppShared.Photos;
using WebAppShared.Types;
using WebAppShared.Utils;

namespace SwipetorApp.Controllers;

public class PostsController(PhotoUrlBuilderSvc photoUrlBuilderSvc, IHostnameCx hostnameCx, IDbProvider dbProvider) : Controller
{
    [Route("/p/{postId:int}")]
    [Route("/p/{postId:int}/{slug}")]
    public IActionResult Post(string infSlug, int postId, string postSlug)
    {
        using var db = dbProvider.Create();
        var post = db.Posts.Include(p => p.User).Include(p => p.Medias).ThenInclude(m => m.Video)
            .Include(p => p.PostHubs).Include(post => post.Medias).ThenInclude(postMedia => postMedia.PreviewPhoto)
            .SingleOrDefault(p => p.Id == postId);

        if (post == null) return RedirectPermanent("/");

        // Only admins and posters can see a deleted post.
        if (post.IsRemoved) return RedirectPermanent("/");

        if (HttpContext.Request.Path.ToString() != post.GetRelativeUrl())
            return RedirectPermanent(post.GetRelativeUrl());
        
        var video = post.Medias.FirstOrDefault()?.Video;
        var desc = video?.Captions;

        if (string.IsNullOrWhiteSpace(desc))
        {
            desc = string.IsNullOrWhiteSpace(post.Title) ? post.Medias.FirstOrDefault()?.Description : post.Title;
        }
        desc = desc.Shorten(157);

        return View("~/Views/Home/Appshell.cshtml", new AppshellViewModel
        {
            Title = $"{post.Title} by @{post.User?.Username}",
            Description = desc.Trim(),
            Image = photoUrlBuilderSvc.GetFullUrl(post.Medias.MinBy(m => m.Ordering)?.PreviewPhoto),
            OpenGraphType = OpenGraphType.VideoOther,
            Url = $"https://{hostnameCx.SiteHostname}{post.GetRelativeUrl()}"
        });
    }
}