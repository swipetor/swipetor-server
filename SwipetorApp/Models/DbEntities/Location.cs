using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using NpgsqlTypes;
using SwipetorApp.Models.Enums;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;
using WebLibServer.Types;

namespace SwipetorApp.Models.DbEntities;

[UsedImplicitly]
public class Location : IDbEntity
{
    public int Id { get; set; }

    [IndexColumn]
    [MaxLength(64)]
    public string Name { get; set; }

    [IndexColumn]
    [MaxLength(64)]
    public string NameAscii { get; set; }

    [IndexColumn]
    [MaxLength(64)]
    public string FullName { get; set; }

    public NpgsqlTsVector SearchVector { get; set; }

    [IndexColumn]
    public double Lat { get; set; }

    [IndexColumn]
    public double Lng { get; set; }

    public int Population { get; set; }

    [MaxLength(256)]
    public string Capital { get; set; }

    [IndexColumn]
    [MaxLength(2)]
    public string Iso2 { get; set; }

    [IndexColumn]
    [MaxLength(3)]
    public string Iso3 { get; set; }

    [MaxLength(256)]
    public string SimplemapsId { get; set; }

    [IndexColumn]
    public LocationType Type { get; set; }

    [IndexColumn]
    public int? ParentId { get; set; }

    [CanBeNull]
    public virtual Location Parent { get; set; }

    public virtual List<Location> Children { get; set; }
}