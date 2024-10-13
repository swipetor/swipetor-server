using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using FFMpegCore;
using SwipetorAppTest.TestLibs;
using WebLibServer.Videos;
using Xunit;
using Xunit.Abstractions;

namespace SwipetorAppTest;

public class VideoPreviewTest(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public async Task VideoPreviewGenerateTest()
    {
        var videoPath = Environment.CurrentDirectory + "/../../../_Data/SampleVideo-1m40s.mp4";
        var videoAnalyzer = new VideoAnalyzer(videoPath);
        var vs = videoAnalyzer.Analysis.PrimaryVideoStream;

        string previewPhotoPath = null;

        try
        {
            previewPhotoPath = Path.GetTempPath() + Guid.NewGuid() + ".png";

            await FFMpeg.SnapshotAsync(videoPath, previewPhotoPath, new Size(vs.Width, vs.Height),
                TimeSpan.FromSeconds(1));

            var fileInfo = new FileInfo(previewPhotoPath);

            Assert.True(fileInfo.Exists);
            Assert.True(fileInfo.Length > 1000);
        }
        finally
        {
            if (previewPhotoPath != null) File.Delete(previewPhotoPath);
        }
    }
}