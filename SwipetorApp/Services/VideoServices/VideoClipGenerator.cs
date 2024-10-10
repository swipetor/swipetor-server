using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebAppShared.Disk;
using WebAppShared.Utils;
using WebAppShared.Videos;

namespace SwipetorApp.Services.VideoServices;

/// <summary>
///     Generates clips from a video file and merges the clips into a single video file.
///     Provide a ScopedTempPath to the constructor so the work dir is not removed before the merged clip can be read.
/// </summary>
/// <param name="tempPath"></param>
public class VideoClipGenerator(ScopedTempPath tempPath)
{
    private readonly ILogger<VideoClipGenerator> _logger =
        AppDefaults.LoggerFactory.Value.CreateLogger<VideoClipGenerator>();

    public async Task<string> GenerateClipsAndMerge(string videoFilePath, List<List<double>> clipTimes)
    {
       _logger.LogInformation("Generating clips from {VideoFilePath} with clip times: {ClipTimes}", videoFilePath,
    string.Join(", ", clipTimes.Select(ct => $"[{string.Join(", ", ct)}]")));
       
       var videoAnalyzer = new VideoAnalyzer(videoFilePath);
       var vs = videoAnalyzer.Analysis.PrimaryVideoStream;
       var originalWidth = vs.Width;
       var originalHeight = vs.Height;
       var targetWidth = (int)(originalHeight * (9.0 / 16.0));
       var fullLength = vs.Duration.TotalSeconds;

        var clipsList = new List<string>();
        var index = 0;

        // Generate clips
        foreach (var ct in clipTimes)
        {
            var clipFilePath = $"clip_{index}.mp4";
            var startTime = ct[0];
            var endTime = Math.Min(ct[1] + 0.3, fullLength); // Add 0.3 seconds to the end time to be similar to html video player
            var duration = endTime - startTime;
            var percentageOffset = ct.Count > 2 ? ct[2] : 0;
            
            var cropXOffset = (int)((originalWidth - targetWidth) / 2 + (originalWidth * percentageOffset / 100));

            _logger.LogInformation(
                "Generating clip {ClipFilePath} from {VideoFilePath} with start {Start} and duration {Duration}",
                clipFilePath, videoFilePath, startTime, duration);

            var ffmpeg = new Ffmpeg(tempPath);

            // Adjusted FFmpeg arguments
            ffmpeg.Arg("ss", ct.First().ToString()) // Set the start time
                .Arg("i", videoFilePath) // Input file
                .Arg("t", duration.ToString()) // Duration of the clip
                .Arg("vf", $"crop={targetWidth}:{originalHeight}:{cropXOffset}:0") // Crop to 9:16 aspect ratio
                // .Arg("c", "copy") // Tells not to encode. Less accurate but faster with this.
                .Arg("map", "0")
                .Arg("avoid_negative_ts", "make_zero")
                // .Arg("copyts", "")
                .Arg("reset_timestamps", "1");

            await ffmpeg.Dest(clipFilePath).ExecAsync();

            clipsList.Add(clipFilePath);
            index++;
        }

        // Merge clips
        var clipListFile = "filelist.txt";
        await using (var fileStream = new StreamWriter(Path.Join(tempPath, clipListFile)))
        {
            _logger.LogInformation("Writing clip list file {ClipListFile}", clipListFile);
            foreach (var file in clipsList) await fileStream.WriteLineAsync($"file '{file}'");

            await fileStream.FlushAsync();
        }

        var mergedFilePath = "merged_clip.mp4";

        _logger.LogInformation("Merging clips into {MergedFilePath}", Path.Join(tempPath, mergedFilePath));

        // MP4Box -cat clip_0.mp4 -cat clip_1.mp4 -new output.mp4
        await ExecuteCmd.RunAsync("MP4Box", $"-cat {string.Join(" -cat ", clipsList)} -new {mergedFilePath}", _logger,
            tempPath);

        /*
         // ffmpeg without re-encoding pauses the playback at merge points, so we use mp4box above.

         var mergeFFmpegCmd = new Ffmpeg(tempPath);
        await mergeFFmpegCmd.Arg("f", "concat").Arg("safe", "0").Arg("i", clipListFile).Arg("c", "copy")
            .Arg("movflags", "+faststart").Dest(mergedFilePath).ExecAsync();

        Cleanup temp files
        foreach (var file in clipsList) File.Delete(Path.Join(tempPath, file));

        File.Delete(clipListFile);

        await ExecuteCmd.RunAsync("MP4Box", $"-add {mergedFilePath} -new fixed_{mergedFilePath}", _logger, tempPath);

        File.Delete(Path.Join(tempPath, mergedFilePath));*/

        return Path.Join(tempPath, mergedFilePath);
    }
}