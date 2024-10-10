using System;

namespace SwipetorApp.System.Extensions;

public class RedirectException(string redirectUrl, bool isPermanent = true) : Exception
{
    public string RedirectUrl { get; } = redirectUrl;

    public bool IsPermanent { get; } = isPermanent;

    public override string Message => $"Redirect to: {RedirectUrl}";
}