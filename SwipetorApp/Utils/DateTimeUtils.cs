using System;
using JetBrains.Annotations;

namespace SwipetorApp.Utils;

[UsedImplicitly]
public static class DateTimeUtils
{
    public static DateTime GetRandomDateTimeWithinLastSixMonths()
    {
        var now = DateTime.UtcNow;
        var sixMonthsAgo = now.AddMonths(-6);
        var random = new Random();
        var range = (now - sixMonthsAgo).Days;
        return sixMonthsAgo.AddDays(random.Next(range)).AddHours(random.Next(24)).AddMinutes(random.Next(60)).AddSeconds(random.Next(60));
    }
}