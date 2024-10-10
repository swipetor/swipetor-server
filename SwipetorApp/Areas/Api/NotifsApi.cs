using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwipetorApp.Areas.Api.Models;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Models.DTOs;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Contexts;
using SwipetorApp.Services.SqlQueries;

namespace SwipetorApp.Areas.Api;

[ApiController]
[Area(AreaNames.Api)]
[Route("api/notifs")]
[Authorize]
public class NotifsApi(UserCx userCx, IMapper mapper, IDbProvider dbProvider) : Controller
{
    public IActionResult Index()
    {
        using var db = dbProvider.Create();

        // Set all notifications as viewed as we render them to the user.
        db.Notifs.Where(n => n.ReceiverUserId == userCx.Value.Id && n.IsViewed == false)
            .UpdateFromQuery(n => new
            {
                IsViewed = true
            });

        db.Users.Where(u => u.Id == userCx.Value.Id).UpdateFromQuery(n => new
        {
            LastNotifCheckAt = DateTime.UtcNow
        });

        var notifs = db.Notifs.Where(n => n.ReceiverUserId == userCx.Value.Id)
            .OrderByDescending(n => n.CreatedAt).Take(20).SelectForUser()
            .ToList();

        var notifDtos = mapper.Map<List<NotifDto>>(notifs);

        return Json(new
        {
            notifs = notifDtos,
            LastNotifCheckAt = mapper.Map<long>(userCx.Value.LastNotifCheckAt)
        });
    }

    [HttpPost]
    [Route("{notifId}/read")]
    public IActionResult MarkRead(int notifId)
    {
        using var db = dbProvider.Create();
        db.Notifs.Where(n => n.Id == notifId && n.ReceiverUserId == userCx.Value.Id).UpdateFromQuery(n => new
        {
            IsRead = true
        });
        db.SaveChanges();

        return Ok();
    }

    [HttpDelete]
    [Route("{notifId}/read")]
    public IActionResult MarkUnread(int notifId)
    {
        using var db = dbProvider.Create();
        db.Notifs.Where(n => n.Id == notifId && n.ReceiverUserId == userCx.Value.Id).UpdateFromQuery(n => new
        {
            IsRead = false
        });
        db.SaveChanges();

        return Ok();
    }

    [HttpPost("register-push-device")]
    public IActionResult RegisterPushDevice([FromBody] RegisterPushDeviceRequestModel model)
    {
        if (string.IsNullOrEmpty(model.Token)) return BadRequest("Push token was empty.");

        using var db = dbProvider.Create();

        if (db.PushDevices.Any(p => p.Token == model.Token && p.UserId == userCx.Value.Id))
            return Ok();

        db.PushDevices.Add(new PushDevice
        {
            UserId = userCx.Value.Id,
            Token = model.Token,
            LastUsedAt = DateTime.UtcNow
        });
        db.SaveChanges();
        return Ok();
    }

    // [HttpGet("test")]
    // public async Task<IActionResult> Test()
    // {
    //     var devices = _cx.PushDevices.ToList();
    //     await _pushNotifSvc.PushToDevices(devices, new PushNotifPayload
    //     {
    //         Title = "Hello msg",
    //         Body = "This works!",
    //         Url = "/notifications",
    //         Tag = PushNotifTag.NewNotifications,
    //         Icon = "/public/images/forum/forum-dot-256.png"
    //     });
    //     return Ok();
    // }
}