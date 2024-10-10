using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwipetorApp.Areas.Api.Models;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Models.DTOs;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services;
using SwipetorApp.Services.Contexts;
using SwipetorApp.Services.Notifs;
using SwipetorApp.Services.RateLimiter;
using SwipetorApp.Services.RateLimiter.Rules;
using SwipetorApp.Services.SqlQueries;
using WebAppShared.Exceptions;
using WebAppShared.Security;

namespace SwipetorApp.Areas.Api;

[ApiController]
[Area(AreaNames.Api)]
[Route("api/comments")]
public class CommentsApi(IDbProvider dbProvider, IMapper mapper, NotifSvc notifSvc, UserIdCx userIdCx)
    : Controller
{
    [Route("/api/posts/{postId:int}/comments")]
    public IActionResult Index(int postId)
    {
        using var db = dbProvider.Create();
        var comments = db.Comments.Where(c => c.PostId == postId).OrderBy(c => c.LikeCount).ThenBy(c => c.CreatedAt)
            .SelectForUser(userIdCx.ValueOrNull).ToList();

        var commentDtos = mapper.Map<List<CommentDto>>(comments);

        return Json(commentDtos);
    }

    [HttpPost]
    [Authorize]
    [RateLimitFilter<CommentRateLimiter>]
    public IActionResult Post([FromBody] CommentReqModel model)
    {
        if (!ModelState.IsValid) throw new HttpJsonError("Comment should be at least 3 characters.");

        var userId = userIdCx.Value;

        using var db = dbProvider.Create();
        var post = db.Posts.Single(p => p.Id == model.PostId);

        var comment = new Comment
        {
            Txt = Sanitize.EditorText(model.Txt),
            UserId = userId,
            PostId = model.PostId
        };

        db.Users.Where(u => u.Id == userId)
            .UpdateFromQuery(u => new { CommentCount = u.CommentCount + 1 });

        db.Comments.Add(comment);
        db.SaveChanges();

        db.Posts.Where(p => p.Id == model.PostId).UpdateFromQuery(p => new
        {
            CommentsCount = p.CommentsCount + 1
        });

        notifSvc.NewComment(userId, comment.Id, post.Id, post.UserId);

        // TODO Notify mentioned users
        // var mentionedUserIds = new MentionExtractor(model.Txt).ExtractUserIds();
        // _notifSvc.NewMentionInComment(_current.User.Id, comment.Id, model.PostId, mentionedUserIds);
        db.SaveChanges();

        var commentQM = db.Comments.Where(c => c.Id == comment.Id).SelectForUser(userId).ToList().Single();

        return Json(mapper.Map<CommentDto>(commentQM));
    }
}