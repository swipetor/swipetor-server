using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwipetorApp.Models.Enums;
using SwipetorApp.Models.Extensions;
using SwipetorApp.Services.Contexts;
using WebLibServer.DI;
using WebLibServer.SharedLogic.Sitemaps;
using WebLibServer.Types;

namespace SwipetorApp.Areas.HostMaster;

[Area(AreaNames.HostMaster)]
public class BatchController(
    IFactory<SitemapGenerator> sitemapGeneratorFactory,
    IHostnameCx hostnameCx,
    IDbProvider dbProvider)
    : Controller
{
    public IActionResult Index()
    {
        return Content("Index Works");
    }
    
    public IActionResult HelloAta()
    {
        return Content("World Works");
    }
    
    public async Task<IActionResult> GenerateSitemaps()
    {
        await using var db = dbProvider.Create();
        using var sitemapGen = sitemapGeneratorFactory.GetInstance();

        sitemapGen.ClearOldSitemaps();
        
        var allUsers = db.Users.Where(u => u.Posts.Any(p => p.IsPublished)).Select(u => new
        {
            user = u,
            lastPostDate = u.Posts.Max(p => (DateTime?)p.CreatedAt)
        }).ToList();

        foreach (var r in allUsers)
            await sitemapGen.AddItem(new SitemapItem
            {
                Loc = $"https://{hostnameCx.SiteHostname}/u/{r.user.Id}/{r.user.Username?.ToLowerInvariant()}",
                Priority = 0.5M,
                ChangeFreq = SitemapItemChangeFreq.Weekly,
                LastMod = r.lastPostDate
            });

        var allPosts = db.Posts.Include(p => p.User)
            .Include(p => p.Medias).ThenInclude(m => m.Video)
            .Where(p => p.IsPublished).ToList();

        foreach (var p in allPosts)
            await sitemapGen.AddItem(new SitemapItem
            {
                Loc = $"https://{hostnameCx.SiteHostname}{p.GetRelativeUrl()}",
                Priority = 0.5M,
                ChangeFreq = SitemapItemChangeFreq.Monthly,
                LastMod = p.ModifiedAt
            });

        await sitemapGen.WriteSitemapFooter();

        await sitemapGen.GenerateSitemapIndex();

        return Content("Sitemap is generated.");
    }
}