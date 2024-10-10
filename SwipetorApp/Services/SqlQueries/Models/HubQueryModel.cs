using JetBrains.Annotations;
using SwipetorApp.Models.DbEntities;

namespace SwipetorApp.Services.SqlQueries.Models;

public class HubQueryModel
{
    public Hub Hub { get; set; }

    [CanBeNull]
    public Photo Photo { get; set; }
}