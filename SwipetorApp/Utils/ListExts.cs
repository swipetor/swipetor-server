using System.Collections.Generic;
using System.Linq;

namespace SwipetorApp.Utils;

public static class ListExts
{
    /// <summary>
    /// Checks if two lists of lists are equal.
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool IsEqualList<T>(this List<List<T>> first, List<List<T>> second)
    {
        if (first == null && second == null)
            return true;

        if (first == null || second == null)
            return false;

        if (first.Count != second.Count)
            return false;

        for (int i = 0; i < first.Count; i++)
        {
            if (!first[i].SequenceEqual(second[i]))
                return false;
        }

        return true;
    }
}