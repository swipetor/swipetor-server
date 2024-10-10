using System.Collections.Generic;
using System.Linq;
using SwipetorApp.Models.DbEntities;

namespace SwipetorApp.Models.Extensions;

public static class VideoExtensions
{
    public static IEnumerable<string> GetFileNames(this Video src)
    {
        return src.Formats.Select(format => $"{src.Id}-{format.Name}.{src.Ext}");
    }
}