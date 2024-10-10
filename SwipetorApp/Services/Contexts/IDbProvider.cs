using SwipetorApp.Models.DbEntities;

namespace SwipetorApp.Services.Contexts;

public interface IDbProvider
{
    DbCx Create();
}