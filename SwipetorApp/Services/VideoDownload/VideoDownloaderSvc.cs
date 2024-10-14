using System;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using WebLibServer.Disk;
using WebLibServer.Utils;
using WebLibServer.WebSys.DI;

namespace SwipetorApp.Services.VideoDownload;

[Service]
[UsedImplicitly]
public class VideoDownloaderSvc(ILogger<VideoDownloaderSvc> logger)
{
    private readonly string[] _ytDlpArgs =
        ["-f \"bestvideo[ext=mp4][vcodec^=avc1][height<=2160]+bestaudio[ext=m4a]/best[ext=mp4][vcodec^=avc1][height<=2160]\"", "--max-filesize 2048m", "-o video.mp4"];

    public async Task<string> Download(string url, ScopedTempPath tempPath)
    {
        logger.LogInformation("Downloading video from {Url}", url);
        await ExecuteCmd.RunAsync("yt-dlp", string.Join(" ", _ytDlpArgs) + " " + url, logger, tempPath);

        return Path.Join(tempPath, "video.mp4");
    }
}