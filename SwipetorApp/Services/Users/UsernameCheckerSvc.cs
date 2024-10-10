using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using SwipetorApp.Services.Contexts;
using WebAppShared.Exceptions;
using WebAppShared.WebSys.DI;

namespace SwipetorApp.Services.Users;

[Service]
[UsedImplicitly]
public class UsernameCheckerSvc(IDbProvider dbProvider)
{
    public void CheckAndThrowIfInvalid(string username)
    {
        if (username.Length < 3) throw new HttpJsonError("Username is too short, 3 characters min.");

        if (username.Length > 18) throw new HttpJsonError("Username is too long, 18 characters max.");

        if (!IsValidUsername(username))
            throw new HttpJsonError("Only use alphanumeric characters, numbers and dot in username.");

        if (!IsAvailable(username))
            throw new HttpJsonError("Username is taken.");
    }

    public bool IsValidUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return false;

        if (username.Length < 3) return false;

        var regex = new Regex("^[a-zA-Z0-9][a-zA-Z0-9.]+[a-zA-Z0-9]$");

        if (!regex.IsMatch(username)) return false;

        return true;
    }

    public bool IsAvailable(string username)
    {
        using var db = dbProvider.Create();

        return !db.Users.Any(u => u.Username == username);
    }
}