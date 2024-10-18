using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SwipetorApp.Models.EmailViewModels;
using SwipetorApp.Models.Enums;
using SwipetorApp.Models.Extensions;
using SwipetorApp.Models.ViewModels;
using SwipetorApp.Services.Auth;
using SwipetorApp.Services.Config;
using SwipetorApp.Services.Contexts;
using SwipetorApp.Services.Emailing;
using SwipetorApp.System.MvcFilters;
using WebLibServer.Contexts;
using WebLibServer.Photos;
using WebLibServer.Types;
using WebLibServer.Utils;

namespace SwipetorApp.Controllers;

public class HomeController(
    IOptions<SiteConfig> siteConfig,
    UserCx userCx,
    IDbProvider dbProvider,
    IHostnameCx hostnameCx,
    EmailerProvider emailerProvider,
    PhotoUrlBuilderSvc photoUrlBuilderSvc)
    : Controller
{
    [Route("/")]
    public IActionResult Index(int? hubId)
    {
        if (hubId != null) return ViewHub(hubId.Value);

        var url = "https://" + hostnameCx.RequestedHostname;

        string title = $"{siteConfig.Value.Name} - {siteConfig.Value.Slogan}";
        string desc = siteConfig.Value.Description;
        string img = $"{url}/public/swipetor/logo-underlined-blackbg-256x256.png";
        
        return View("Appshell", new AppshellViewModel
        {
            Title = title,
            Description = desc,
            Image = img,
            OpenGraphType = OpenGraphType.Website,
            Url = url
        });
    }

    public IActionResult ViewHub(int hubId)
    {
        using var db = dbProvider.Create();
        var hub = db.Hubs.SingleOrDefault(c => c.Id == hubId);

        if (hub == null) return Redirect("/");

        return View("Appshell", new AppshellViewModel
        {
            Title = $"{hub.Name} posts - {siteConfig.Value.Name}",
            Description =
                $"{hub.Name} hub lists related videos and posts. {siteConfig.Value.Name} - {siteConfig.Value.Slogan}",
            Image = "/public/swipetor/logo-underlined-blackbg-256x256.png",
            OpenGraphType = OpenGraphType.Website,
            Url = $"https://{siteConfig.Value.Hostname}/?hubId={hubId}"
        });
    }

    [Route("/login")]
    [Route("/login/code")]
    public IActionResult PublicBoardRoutes()
    {
        return View("Appshell", new AppshellViewModel());
    }

    [Route("/pm")]
    [Route("/pm/new")]
    [Route("/pm/{threadId:int}")]
    [Route("/my")]
    [Route("/my/first-login")]
    [Route("/post-builder")]
    [Route("/post-builder/{postId:int}")]
    [Route("/notifs")]
    [Route("/become-creator")]
    [Route("/sub-plans")]
    [Route("/favs")]
    public IActionResult LoggedOnlyRoutes()
    {
        if (!userCx.IsLoggedIn) return Redirect($"/login?redir={Request.Path}{Request.QueryString}");

        return View("Appshell", new AppshellViewModel());
    }

    /*[Route("/p/{postId:int}")]
    [Route("/p/{postId:int}/{slug}")]
    [ProcessLinkRef]
    public IActionResult ViewPost(int postId, string slug, int? hubId = null)
    {
        using var db = dbProvider.Create();
        var post = db.Posts.Include(p => p.User).Include(p => p.Influencer)
            .Include(p => p.PostHubs).Include(post => post.Medias).ThenInclude(postMedia => postMedia.PreviewPhoto)
            .SingleOrDefault(p => p.Id == postId);

        if (post == null) return RedirectPermanent("/");

        // Only admins and posters can see a deleted post.
        if (post.IsRemoved) return RedirectPermanent("/");

        if (string.IsNullOrWhiteSpace(slug) || slug.Trim() != post.GetSlug())
            return RedirectPermanent(post.GetRelativeUrl());

        var desc = !string.IsNullOrWhiteSpace(post.Title) ? post.Title : post.Medias.FirstOrDefault()?.Description;
        desc = desc.Shorten(157);

        return View("Appshell", new AppshellViewModel
        {
            Title = $"{post.Title} by @{post.Influencer?.Name ?? post.User?.Username}",
            Description = desc.Trim(),
            Image = photoUrlBuilderSvc.GetFullUrl(post.Medias.MinBy(m => m.Ordering)?.PreviewPhoto),
            OpenGraphType = OpenGraphType.VideoOther,
            Url = $"https://{siteConfig.Value.Hostname}{post.GetRelativeUrl()}"
        });
    }*/
    
    [Route("/u/{userId:int}/{userName?}")]
    public IActionResult UserPage(int userId, string userName)
    {
        using var db = dbProvider.Create();
        var user = db.Users.Include(u => u.Photo).SingleOrDefault(u => u.Id == userId);

        if (user == null) return RedirectPermanent("/");

        var desc = user.Description ?? siteConfig.Value.UserProfileDesc.Replace("{username}", user.Username);
        var title = siteConfig.Value.UserProfileTitle.Replace("{username}", user.Username);

        return View("Appshell", new AppshellViewModel
        {
            Title = title,
            Description = desc.Trim(),
            Image = user.Photo != null ? photoUrlBuilderSvc.GetFullUrl(user.Photo) : "/public/images/nophoto/nophoto-600.png",
            OpenGraphType = OpenGraphType.Profile,
            Url = $"https://{siteConfig.Value.Hostname}/u/{userId}/{user.Username}"
        });
    }

    [HttpGet(@"/sw.js")]
    public IActionResult SwJs()
    {
        return File("public/build/sw.js", "text/javascript");
    }

    [HttpGet(@"/manifest.json")]
    public IActionResult ManifestJson()
    {
        Response.ContentType = "application/json";
        return View("ManifestJson");
    }

    [HttpGet(@"/robots.txt")]
    public IActionResult RobotsTxt()
    {
        Response.ContentType = "text/plain";
        return View("RobotsTxt");
    }

    [HttpGet("/upgrade-creator")]
    [Authorize]
    public IActionResult CreatorUpgrade(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) return Redirect("/");

        var user = userCx.Value;
        
        if (user.Role >= UserRole.Creator) return Redirect("/");
        
        using var db = dbProvider.Create();
        user = db.Users.Single(u => u.Id == user.Id);
        user.Role = UserRole.Creator;
        db.SaveChanges();
        
        //send email notifying atasasmaz@gmail.com
        var txt = $"<p>User: {user.Username} #{user.Id}</p><p>Code: {code}</p>";
        var genericEmailVM = new GenericEmailVM
        {
            Text = txt,
        };

        var emailer = emailerProvider.GetGenericEmailer<GenericEmailVM>();
        emailer.Send(siteConfig.Value.Email, "User Upgraded to Creator", genericEmailVM);
        
        return Redirect("/?msgCode=" + SayMsgKey.UserUpgradedCreator);
    }
}
