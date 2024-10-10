using System.Linq;
using JetBrains.Annotations;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Contexts;
using WebAppShared.Contexts;
using WebAppShared.WebSys.DI;

namespace SwipetorApp.Services.GeoLocation;

[Service]
[UsedImplicitly]
public class GeoLocationSvc(IDbProvider dbProvider, IConnectionCx connectionCx)
{
    [CanBeNull]
    public Location GetDefaultLocation()
    {
        using var db = dbProvider.Create();

        var countryIso2 = connectionCx.IpAddressCountry;

        var country = db.Locations.Where(l => l.Iso2 == countryIso2 && l.Type == LocationType.Country).FirstOrDefault();

        if (country == null) return null;

        var biggestCity = db.Locations.Where(l =>
                l.ParentId == country.Id || (l.Parent.ParentId == country.Id && l.Type == LocationType.City))
            .OrderByDescending(l => l.Population).FirstOrDefault();

        return biggestCity;
    }
}