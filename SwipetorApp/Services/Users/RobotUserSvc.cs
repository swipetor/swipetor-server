using System;
using System.Linq;
using JetBrains.Annotations;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Services.Auth;
using SwipetorApp.Services.Contexts;
using WebAppShared.WebSys.DI;

namespace SwipetorApp.Services.Users;

[Service, UsedImplicitly]
public class RobotUserSvc(IDbProvider dbProvider)
{
    public User GetRandomRobotUser()
    {
        using var db = dbProvider.Create();

        var user = db.Users.Where(u => u.Role == UserRole.Robot).OrderBy(u => Guid.NewGuid())
            .FirstOrDefault();

        return user;
    }
}