using System.ComponentModel;

namespace SwipetorApp.Models.Enums;

public enum OpenGraphType
{
    [Description("place")]
    Place,

    [Description("website")]
    Website,

    [Description("article")]
    Article,

    [Description("product")]
    Product,

    [Description("video.movie")]
    VideoMovie,

    [Description("video.episode")]
    VideoEpisode,

    [Description("video.tv_show")]
    VideoTvShow,

    [Description("video.other")]
    VideoOther,
    
    [Description("profile")]
    Profile
}