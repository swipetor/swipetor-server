using System;
using CloneExtensions;
using SwipetorApp.Models.Enums;

namespace SwipetorApp.System.Binders;

public static class CloningInitializer
{
    public static void Init()
    {
        var cloneableFn = (object source) => source is ICloneable cloneable ? cloneable.Clone() : null;
        
        // CloneFactory.CustomInitializers.Add(typeof(SiteMode), cloneableFn);
    }
}