using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AngleSharp.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Network.Fluent.PCFilter.Definition;
using Microsoft.EntityFrameworkCore;
using SwipetorApp.Areas.Api.Models;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Models.DTOs;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Contexts;
using SwipetorApp.Services.Pm;
using SwipetorApp.Services.SqlQueries;
using SwipetorApp.Services.SqlQueries.Models;
using WebAppShared.Exceptions;
using WebAppShared.Metrics;

namespace SwipetorApp.Areas.Api;

[ApiController]
[Area(AreaNames.Api)]
[Route("api/pm")]
[Authorize]
public class PmApi(
    IMapper mapper,
    IDbProvider dbProvider,
    PmThreadSvc pmThreadSvc,
    PmMsgSenderSvc pmMsgSender,
    UserIdCx userIdCx,
    IMetricsSvc metricsSvc)
    : Controller
{
    /// <summary>
    ///     Get threads of current user
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
        // Update users last PM check time non-blocking
        Task.Run(() =>
        {
            using var db = dbProvider.Create();
            db.Users.Where(u => u.Id == userIdCx).UpdateFromQuery(u => new
            {
                LastPmCheckAt = DateTime.UtcNow
            });
        });

        using var db = dbProvider.Create();
        var threads = db.PmThreads.Where(t => t.ThreadUsers.Any(tu => tu.UserId == userIdCx))
            .OrderByDescending(t => t.LastMsgAt).Take(20).ToList();

        // db.TenantUsers.Where(tu => tu.Role >= UserRole.TenantEditor).Select(tu => tu.User)
        // .Include(u => u.Photo).Include(u => u.TenantUsers).ToList();

        var threadDtos = mapper.Map<List<PmThreadDto>>(threads);

        return Json(new
        {
            threads = threadDtos
        });
    }

    [HttpPost("init")]
    public async Task<IActionResult> Init(PmInitReqModel model)
    {
        if (!ModelState.IsValid) throw new HttpJsonError("Min 1, Max 3 users can be added");
        
        await using var db = dbProvider.Create();
        var thread = pmThreadSvc.GetOrCreateThread(model.UserIds, userIdCx);
        
        return Json(mapper.Map<long>(thread.Id));
    }

    /// <summary>
    ///     Get threads of a user
    ///     TODO Convert to single user threads
    /// </summary>
    /// <param name="userIds"></param>
    /// <returns></returns>
    [HttpGet("threads")]
    public IActionResult Threads(string userIds)
    {
        using var db = dbProvider.Create();
        var userIdArr = userIds.Trim().Split(",").Select(int.Parse).ToList();
        userIdArr.Add(userIdCx);
        userIdArr = userIdArr.Distinct().ToList();

        var pmThreads = db.PmThreads.Where(pt =>
                pt.ThreadUsers.All(ptu => userIdArr.Contains(ptu.UserId)) &&
                pt.ThreadUsers.Count == userIdArr.Count)
            .SelectForUser()
            .ToList();

        return Json(mapper.Map<List<PmThreadDto>>(pmThreads));
    }

    /// <summary>
    ///     Get PM thread
    /// </summary>
    /// <param name="threadId"></param>
    /// <returns></returns>
    [HttpGet("{threadId}")]
    public IActionResult Thread(long threadId)
    {
        using var db = dbProvider.Create();
        var pmThreads = db.PmThreads.Where(pt => pt.Id == threadId)
            .SelectForUser()
            .ToList().SingleOrDefault();

        return Json(mapper.Map<PmThreadDto>(pmThreads));
    }

    /// <summary>
    ///     Get messages of a thread
    /// </summary>
    /// <param name="markedRead"></param>
    /// <param name="threadId"></param>
    /// <returns></returns>
    [HttpGet("{threadId}/msgs")]
    public IActionResult Msgs(bool markedRead, int threadId)
    {
        using var db = dbProvider.Create();

        var msgs = db.PmMsgs
            .Where(m => m.ThreadId == threadId && m.Thread.ThreadUsers.Any(tu => tu.UserId == userIdCx.Value))
            .OrderByDescending(m => m.CreatedAt).Take(20)
            .ToList();
        
        var lastMsgId = msgs.LastOrDefault()?.Id;

        if (markedRead)
            db.PmThreadUsers.Where(ptu => ptu.ThreadId == threadId && ptu.UserId == userIdCx)
                .UpdateFromQuery(p => new
                {
                    FirstUnreadMsgId = (long?)null,
                    LastReadMsgId = lastMsgId,
                    UnreadMsgCount = 0,
                    EmailSentAt = (DateTime?)null
                });

        var msgDtos = mapper.Map<List<PmMsgDto>>(msgs);

        return Json(msgDtos);
    }

    /// <summary>
    ///     Send a message to a thread
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="HttpStatusCodeException"></exception>
    [HttpPost("{threadId}/msgs")]
    public async Task<IActionResult> SendMsg(PmSendMsgRequestModel model)
    {
        await using var db = dbProvider.Create();
        var thread = db.PmThreads.Include(t => t.ThreadUsers).Single(t => t.Id == model.ThreadId);
        if (!thread.ThreadUsers.Any(tu => tu.UserId == userIdCx))
            throw new HttpStatusCodeException(HttpStatusCode.Forbidden, "Can't send msg here");

        var msg = await pmMsgSender.Send(model.ThreadId, model.Txt);

        var msgDto = mapper.Map<PmMsgDto>(msg);

        metricsSvc.CollectNamed(AppEvent.PmMsg);

        return Json(msgDto);
    }
}