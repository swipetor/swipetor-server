using System;
using Moq;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.System.Extensions;
using WebLibServer.Uploaders;
using WebLibServer.Videos;
using Xunit;

namespace SwipetorAppTest.System.Extensions;

public class VideoExtensionsTests
{
    [Fact]
    public void GetHttpUrl_ReturnsExpectedUrl()
    {
        // Arrange
        var video = new Video
        {
            Id = new Guid(),
            Formats = [VideoResolution.X4K]
        };

        var mockBucket = new Mock<IStorageBucket>();
        mockBucket.Setup(b => b.MediaHost).Returns("media.example.com");
        mockBucket.Setup(b => b.Videos).Returns("testVideosPath");

        var resultUrl = video.GetHttpUrl(mockBucket.Object);

        var expectedUrl = $"https://media.example.com/testVideosPath/{video.Id}-{VideoResolution.X4K}.mp4";
        Assert.Equal(expectedUrl, resultUrl);
    }
}