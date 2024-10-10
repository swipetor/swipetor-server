using System;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SwipetorAppTest.TestLibs;
using WebAppShared.Videos;
using Xunit;
using Xunit.Abstractions;

namespace SwipetorAppTest;

public class VideoSpriteGeneratorTest(ITestOutputHelper output) : TestBase(output)
{
    // TODO : Fix this test that has problems with System.Drawing.Common
    [Theory(Skip = "Disabled test due to System.Drawing.Common issues.")]
    [InlineData("SampleVideo-1m40s.mp4", 100, 20, 150, 84)]
    [InlineData("SampleVideo-5m3s.mp4", 303, 50, 150, 84)]
    public async Task GenerateFromSampleVideo(string testVideoFileName, int duration, int imageCount, int thumbWidth,
        int thumbHeight)
    {
        var videoPath = Environment.CurrentDirectory + "/../../../_Data/" + testVideoFileName;
        var videoAnalyzer = new VideoAnalyzer(videoPath);
        var spriteGenerator = new VideoSpriteGenerator(videoPath, videoAnalyzer);

        Assert.Equal(duration, (int)videoAnalyzer.Duration.TotalSeconds);

        Assert.Equal(imageCount, spriteGenerator.CalculateImageCount());

        var spriteResult = await spriteGenerator.Generate();

        Assert.Equal(thumbWidth * Math.Min(50, imageCount), spriteResult.First().Image.Width);
        Assert.Equal(thumbHeight, spriteResult.First().Image.Height);

        spriteResult.First().Image
            .SaveAsJpeg(Environment.CurrentDirectory + "/../../../_Data/" + testVideoFileName + ".jpg");
    }
}