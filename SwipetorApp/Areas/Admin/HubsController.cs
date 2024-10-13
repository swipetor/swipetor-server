using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwipetorApp.Areas.Admin.ViewModels;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Auth;
using SwipetorApp.Services.Contexts;
using SwipetorApp.Services.PhotoServices;
using SwipetorApp.System.UserRoleAuth;
using WebLibServer.DI;
using WebLibServer.Photos;

namespace SwipetorApp.Areas.Admin;

[Area(AreaNames.Admin)]
[UserRoleAuth(UserRole.Admin)]
public class HubsController(
    PhotoDeleterSvc photoDeleterSvc,
    IFactory<PhotoSaverSvc> photoSaverFactory,
    IDbProvider dbProvider)
    : Controller
{
    public IActionResult Index()
    {
        using var db = dbProvider.Create();

        var model = new HubsHomeViewModel
        {
            Hubs = db.Hubs.Include(c => c.Photo).OrderBy(c => c.Name).ToList()
        };

        return View(model);
    }

    public IActionResult Add()
    {
        return View("AddEdit", new HubsAddEditViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Add(HubsAddEditViewModel model)
    {
        if (!ModelState.IsValid) return Add();

        var hub = new Hub
        {
            Name = model.Name.Trim()
        };

        await using var db = dbProvider.Create();

        db.Hubs.Add(hub);
        await db.SaveChangesAsync();

        if (model.HubIcon != null && CommonPhotoUtils.IsExtensionPhotoFile(model.HubIcon.FileName))
        {
            var hubIcon = model.HubIcon;
            using var uploader = photoSaverFactory.GetInstance()
                .SetSource(hubIcon.OpenReadStream())
                .SetExtensionByFileName(hubIcon.FileName);

            var saved = await uploader.Save();

            hub.Photo = saved;
            await db.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }

    public IActionResult Edit(int hubId)
    {
        Hub hub;
        using (var db = dbProvider.Create())
        {
            hub = db.Hubs.Include(l => l.Photo).Single(c => c.Id == hubId);
        }

        return View("AddEdit", new HubsAddEditViewModel
        {
            Id = hub.Id,
            Name = hub.Name,
            Photo = hub.Photo
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(HubsAddEditViewModel model)
    {
        if (!ModelState.IsValid) return View("AddEdit", model);

        await using var db = dbProvider.Create();

        var hub = db.Hubs.Include(c => c.Photo).Single(f => f.Id == model.Id);

        hub.Name = model.Name.Trim();

        await db.SaveChangesAsync();

        if (model.HubIcon != null && CommonPhotoUtils.IsExtensionPhotoFile(model.HubIcon.FileName))
        {
            var hubIcon = model.HubIcon;

            using var uploader = photoSaverFactory.GetInstance()
                .SetSource(hubIcon.OpenReadStream())
                .SetExtensionByFileName(hubIcon.FileName);

            // Delete existing photo if exists
            if (hub.PhotoId != null) uploader.DeletePreviousClause = p => p.Id == hub.PhotoId;

            var saved = await uploader.Save();
            hub.Photo = saved;
            await db.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Delete(int hubId)
    {
        using var db = dbProvider.Create();

        var hub = db.Hubs.Single(c => c.Id == hubId);

        var hubList = db.Hubs
            .Where(c => c.Id != hubId)
            .Select(c => new Tuple<int?, string>(c.Id, c.Name)).ToList();
        hubList.Insert(0, new Tuple<int?, string>(null, "Select a hub"));

        return View(new HubsDeleteViewModel
        {
            Hub = hub,
            HubList = hubList,
            HubIdToDelete = hubId
        });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(HubsDeleteViewModel model)
    {
        if (!ModelState.IsValid) return RedirectToAction("Delete", new { hubId = model.HubIdToDelete });

        var hubIdToDelete = model.HubIdToDelete ?? 0;
        var hubIdToMovePosts = model.HubIdToMovePosts ?? 0;

        await using var db = dbProvider.Create();

        var hub = db.Hubs.Single(c => c.Id == model.HubIdToDelete);

        // Move all posts of the hub being deleted
        await db.PostHubs.Where(pc => pc.HubId == hubIdToDelete &&
                                      pc.Post.PostHubs.All(c => c.HubId != hubIdToMovePosts))
            .UpdateFromQueryAsync(pc =>
                new
                {
                    HubId = hubIdToMovePosts
                });

        // Delete hub's photo if exists
        if (hub.PhotoId != null) await photoDeleterSvc.Delete(hub.PhotoId.Value);

        db.Hubs.Remove(hub);
        await db.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}