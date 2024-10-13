using System.Linq;
using SwipetorApp.Models.DbEntities;
using WebLibServer.Uploaders;

namespace SwipetorApp.System.Extensions;

public static class VideoExtensions
{
    public static string GetHttpUrl(this Video video, IStorageBucket storageBucket) => video == null
        ? null
        : $"https://{storageBucket.MediaHost}/{storageBucket.Videos}/{video.Id}-{video.Formats.First()}.mp4";
}