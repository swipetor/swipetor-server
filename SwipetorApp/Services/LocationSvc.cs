using JetBrains.Annotations;
using WebLibServer.WebSys.DI;

namespace SwipetorApp.Services;

[Service]
[UsedImplicitly]
public class LocationSvc
{
    // public List<Tuple<int, string, Location>> GetDropDownList(int forCountryId)
    // {
    //     var locations = _cx.Locations.Where(l => l.CountryId == forCountryId && l.ParentId == null)
    //         .Include(l => l.Children)
    //         .ThenInclude(c => c.Children).ThenInclude(c => c.Children).OrderBy(l => l.Name).ToList();
    //
    //     var flatLocations = new List<Tuple<int, string, Location>>();
    //
    //     foreach (var location in locations)
    //     {
    //         InsertLocationAndChildren(flatLocations, location, "");
    //     }
    //
    //     return flatLocations;
    // }
    //
    // private void InsertLocationAndChildren(List<Tuple<int, string, Location>> flatLocs, Location loc, string dashes)
    // {
    //     flatLocs.Add(new Tuple<int, string, Location>(loc.Id, $"{dashes} {loc.Name}", loc));
    //
    //     if (loc.Children == null) return;
    //
    //     foreach (var child in loc.Children.OrderBy(c => c.Ordering).ThenBy(c => c.Name))
    //     {
    //         InsertLocationAndChildren(flatLocs, child, dashes + "-");
    //     }
    // }
}