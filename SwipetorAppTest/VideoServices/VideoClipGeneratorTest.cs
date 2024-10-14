using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SwipetorApp.Services.VideoServices;
using SwipetorAppTest.TestLibs;
using WebLibServer.Disk;
using WebLibServer.Videos;
using Xunit;
using Xunit.Abstractions;

namespace SwipetorAppTest.VideoServices;

public class VideoClipGeneratorTest(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public async Task GenerateClipsAndMerge_Successful()
    {
        var tempPath = new ScopedTempPath();

        // Arrange
        var videoClipGenerator = new VideoClipGenerator(tempPath);
        var videoFilePath = Environment.CurrentDirectory + "/../../../_Data/SampleVideo-5m3s.mp4";
        var clipTimes = new List<List<double>>
        {
            new() { 30, 40 },
            new() { 150, 170 }
        };

        // Act
        var result = await videoClipGenerator.GenerateClipsAndMerge(videoFilePath, clipTimes);

        // Assert
        Assert.NotNull(result);
        Assert.True(File.Exists(result));

        var fileInfo = new VideoAnalyzer(result);

        Assert.Equal(30, fileInfo.Duration.Seconds);
    }
}