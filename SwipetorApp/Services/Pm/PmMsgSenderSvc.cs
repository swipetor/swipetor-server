using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Services.Config;
using SwipetorApp.Services.Contexts;
using WebAppShared.Exceptions;
using WebAppShared.WebSys.DI;

namespace SwipetorApp.Services.Pm;

[Service]
[UsedImplicitly]
public class PmMsgSenderSvc(
    UserCx userCx,
    PmInstantWebPushSvc pmInstantWebPushSvc,
    ILogger<PmMsgSenderSvc> logger,
    IDbProvider dbProvider)
{
    public async Task<PmMsg> Send(long threadId, string msgTxt)
    {
        await using var db = dbProvider.Create();
        logger.LogInformation("Sending a new msg to thread {}", threadId);

        var thread = db.PmThreads
            .Include(pt => pt.ThreadUsers)
            .ThenInclude(tu => tu.User)
            .Single(pt => pt.Id == threadId);
        
        if (thread.ThreadUsers.All(tu => tu.UserId != userCx.Value.Id))
            throw new HttpJsonError("You are not a member of this thread");
        
        var pmMsg = new PmMsg
        {
            Txt = msgTxt.Trim(),
            UserId = userCx.Value.Id,
            ThreadId = thread.Id,
            ThreadUserId = thread.ThreadUsers.Single(tu => tu.UserId == userCx.Value.Id).Id 
        };
        db.PmMsgs.Add(pmMsg);
        await db.SaveChangesAsync();
        
        foreach (var ptu in thread.ThreadUsers)
            if (ptu.UserId == userCx.Value.Id)
            {
                ptu.LastMsgAt = DateTime.UtcNow;
                ptu.LastReadMsg = pmMsg;
            }
            else
            {
                ptu.UnreadMsgCount++;
                ptu.FirstUnreadMsgId ??= pmMsg.Id;
            }

        thread.LastMsgId = pmMsg.Id;
        thread.LastMsgAt = DateTime.UtcNow;
        
        await db.SaveChangesAsync();

#pragma warning disable CS4014
        Task.Run(() => pmInstantWebPushSvc.TrySendForPmMsgId(pmMsg.Id));
#pragma warning restore CS4014

        return pmMsg;
    }
}