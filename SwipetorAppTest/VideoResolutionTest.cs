using WebAppShared.Videos;
using Xunit;

namespace SwipetorAppTest;

public class VideoResolutionTest
{
    [Fact]
    public void TestVideoResolution()
    {
        var list = VideoResolution.GetList();

        Assert.Equal(9, list.Count);

        Assert.Equal(VideoResolution.X360P, VideoResolution.GetDictionary()["360p"]);
        Assert.Equal(VideoResolution.X480P, VideoResolution.GetDictionary()["480p"]);
        Assert.Equal(VideoResolution.X720P, VideoResolution.GetDictionary()["720p"]);

        Assert.Equal(VideoResolution.X1440P, (VideoResolution)"1440p");
        Assert.Equal("2160p", (string)VideoResolution.X2160P);
        Assert.Equal("4K", VideoResolution.X4K.ToString());
    }
}