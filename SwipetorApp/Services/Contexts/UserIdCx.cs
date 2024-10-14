using System.Net;
using JetBrains.Annotations;
using WebLibServer.Exceptions;
using WebLibServer.WebSys.DI;

namespace SwipetorApp.Services.Contexts;

[Service(ServiceAttribute.Scopes.Scoped)]
[UsedImplicitly]
[CanBeNull]
public class UserIdCx(UserCx userCx)
{
    /// <summary>
    ///     Accessing the user id via this property will throw an exception if user id is null.
    /// </summary>
    public int Value => userCx.ValueOrNull?.Id ?? throw new HttpStatusCodeException(HttpStatusCode.Unauthorized);

    public int? ValueOrNull => userCx.ValueOrNull?.Id;

    public bool IsNull()
    {
        return userCx.ValueOrNull == null;
    }

    public static implicit operator int?(UserIdCx value)
    {
        return value.ValueOrNull;
    }

    public static implicit operator int(UserIdCx value)
    {
        return value.Value;
    }
}