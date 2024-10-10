using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SwipetorApp.Services.Config.UIConfigs;

public class UIConfigContractResolver : CamelCasePropertyNamesContractResolver
{
    private static readonly ConcurrentDictionary<Type, IList<JsonProperty>> Cache = new();


    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        // Try to get properties from the cache
        if (Cache.TryGetValue(type, out var cachedProperties))
            return cachedProperties;

        var properties = base.CreateProperties(type, memberSerialization);
        IList<JsonProperty> filteredProperties = properties.Where(p =>
            p.AttributeProvider != null &&
            p.AttributeProvider.GetAttributes(typeof(OutputConfigToUIAttribute), true).Any()
        ).ToList();

        // Store the properties in the cache
        Cache[type] = filteredProperties;

        return filteredProperties;
    }
}