using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using SwipetorApp.Models.DbEntities;
using WebAppShared.Contexts;
using WebAppShared.WebSys.DI;

namespace SwipetorApp.Services.Contexts;

[Service(InterfaceToBind = typeof(IDbProvider))]
[UsedImplicitly]
public class DbProvider(IConnectionCx connectionCx, DbContextOptions<DbCx> options) : IDbProvider
{
    public DbCx Create()
    {
        return new DbCx(connectionCx, options);
    }
}