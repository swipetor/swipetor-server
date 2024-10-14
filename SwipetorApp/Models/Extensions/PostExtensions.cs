using SwipetorApp.Models.DbEntities;
using WebLibServer.Http;

namespace SwipetorApp.Models.Extensions;

public static class PostExtensions
{
    public static string GetRelativeUrl(this Post post)
    {
        return $"/p/{post.Id}/{post.GetSlug()}";
    }

    public static string GetSlug(this Post post)
    {
        return UrlUtils.ConvertToSlug(post.Title) + "-by-" + UrlUtils.ConvertToSlug(post.User?.Username);
    }
}