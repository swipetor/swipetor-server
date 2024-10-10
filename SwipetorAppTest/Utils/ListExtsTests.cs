using System;
using System.Collections.Generic;
using SwipetorApp.Utils;
using Xunit;

namespace SwipetorAppTest.Utils;

public class ListExtsTests
{
    public static IEnumerable<object[]> GetTestData()
    {
        yield return Data(null, null, true);
        yield return Data(null, [[1,2]], false);
        yield return Data([[1,2]], [[3,4]], false);
        yield return Data([[1,2],[3,4]], [[1,2],[3,4]], true);
        yield return Data([[0,1],[1,2],[3,4]], [[0,1],[1,2],[3,4]], true);
        yield return Data([[0,1],[1,2],[3,4]], [[1,2],[3,4],[0,1]], false);
        yield break;

        object[] Data(List<List<object>> a, List<List<object>> b, bool result) => [a, b, result];
    }
    
    [Theory]
    [MemberData(nameof(GetTestData))]
    public void IsEqualList_BothNull_ReturnsTrue<T>(List<List<T>> first, List<List<T>> second, bool expected)
    {
        Assert.Equal(first.IsEqualList(second), expected);
    }
}