using System;
using System.Collections.Generic;
using System.Linq;

namespace SwipetorApp.System.Extensions;

public static class CollectionExts
{
    public static void UpdateCollection<T>(this ICollection<T> source, IEnumerable<int> newIds, 
        Func<int, T> createNewItem, Func<T, int> getId) 
        where T : class
    {
        // Find items to remove
        var itemsToRemove = source.Where(item => !newIds.Contains(getId(item))).ToList();

        // Remove items that are not in the new list
        foreach (var item in itemsToRemove)
        {
            source.Remove(item);
        }

        // Find items to add
        var existingIds = source.Select(getId).ToHashSet();
        var itemsToAdd = newIds.Where(id => !existingIds.Contains(id))
            .Select(createNewItem)
            .ToList();

        // Add new items
        foreach (var item in itemsToAdd)
        {
            source.Add(item);
        }
    }
}