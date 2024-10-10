using System.Collections.Generic;
using Newtonsoft.Json;

namespace SwipetorApp.Models;

public class ManifestJsonModel
{
    public string Name { get; set; }
    public string ShortName { get; set; }
    public string StartUrl { get; set; } = "/?utm_source=homescreen";
    public string Display { get; set; } = "standalone";
    public string Orientation { get; set; } = "portrait";

    [JsonProperty("theme_color")]
    public string ThemeColor { get; set; }

    [JsonProperty("background_color")]
    public string BackgroundColor { get; set; }

    public List<Icon> Icons { get; set; }

    public class Icon
    {
        public string Src { get; set; }
        public string Sizes { get; set; }
        public string Type { get; set; }
        public double Density { get; set; }
    }
}