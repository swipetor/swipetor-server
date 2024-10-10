using System;
using System.Collections.Generic;
using MoreLinq.Extensions;

namespace SwipetorApp.Utils;

public static class MultiValueDictionaryExtensions
{
    /// <summary>
    ///     Adds the specified value to the multi value dictionary.
    /// </summary>
    /// <param name="thisDictionary"></param>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
    public static void Add<TKeyType, TListType, TValueType>(this Dictionary<TKeyType, TListType> thisDictionary,
        TKeyType key, TValueType value)
        where TListType : IList<TValueType>, new()
    {
        //if the dictionary doesn't contain the key, make a new list under the key
        if (!thisDictionary.ContainsKey(key)) thisDictionary.Add(key, []);

        //add the value to the list at the key index
        thisDictionary[key].Add(value);
    }

    public static MultiValueDictionary<TKey, TValue> ToMultiValueDictionary<T, TKey, TValue>(
        this IEnumerable<T> source, Func<T, TKey> selector, Func<T, TValue> valueSelector)
    {
        var multiDict = new MultiValueDictionary<TKey, TValue>();
        source.ForEach(s => multiDict.Add(selector(s), valueSelector(s)));
        return multiDict;
    }
}