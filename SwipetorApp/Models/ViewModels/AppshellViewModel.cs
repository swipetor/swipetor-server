using SwipetorApp.Models.Enums;
using WebAppShared.WebSys;

namespace SwipetorApp.Models.ViewModels;

public class AppshellViewModel
{
    private readonly bool _isDevelopment = AppEnv.IsDevelopment;

    public string Title { get; set; }

    public string PublicDir =>
        "public" + (!_isDevelopment ? $"{DeployInfo.GetAppVersion()}-{DeployInfo.GetUiVersion()}" : "");

    public string Description { get; set; }

    public string Url { get; set; }

    public OpenGraphType? OpenGraphType { get; set; }

    public string Image { get; set; }

    public int? ImageWidth { get; set; }
    public int? ImageHeight { get; set; }
}