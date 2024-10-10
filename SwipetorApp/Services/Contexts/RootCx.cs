using JetBrains.Annotations;
using SwipetorApp.Models.DbEntities;
using WebAppShared.Contexts;
using WebAppShared.WebSys.DI;

namespace SwipetorApp.Services.Contexts;

[Service(ServiceAttribute.Scopes.Scoped)]
[UsedImplicitly]
public class RootCx(UserCx userCx, IConnectionCx connectionCx)
{
    public User User => userCx.Value;

    [CanBeNull]
    public User UserOrNull => userCx.ValueOrNull;

    /// <summary>
    ///     If the current request has a valid logged in user
    /// </summary>
    public bool IsLoggedIn => userCx.IsLoggedIn;

    public string IpAddress => connectionCx.IpAddress;

    public string IpAddressCountry => connectionCx.IpAddressCountry;

    public string BrowserAgent => connectionCx.BrowserAgent;
}