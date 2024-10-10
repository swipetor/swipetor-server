using SwipetorApp.Models.Enums;

namespace SwipetorApp.Models.DTOs;

public class LocationDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string NameAscii { get; set; }

    public string FullName { get; set; }

    public double Lat { get; set; }

    public double Lng { get; set; }

    public string Iso2 { get; set; }

    public string Iso3 { get; set; }

    public LocationType Type { get; set; }

    public int? ParentId { get; set; }
}