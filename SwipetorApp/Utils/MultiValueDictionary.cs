using System.Collections.Generic;

namespace SwipetorApp.Utils;

public class MultiValueDictionary<TKeyType, TValueType> : Dictionary<TKeyType, List<TValueType>>
{
    /// <summary>
    ///     Hide the regular Dictionary Add method
    /// </summary>
    private new void Add(TKeyType key, List<TValueType> value)
    {
        base.Add(key, value);
    }

    /// <summary>
    ///     Adds the specified value to the multi value dictionary.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
    public void Add(TKeyType key, TValueType value)
    {
        //add the value to the dictionary under the key
        MultiValueDictionaryExtensions.Add(this, key, value);
    }
}