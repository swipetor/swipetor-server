using Microsoft.Extensions.Logging;
using WebAppShared.Types;

namespace SwipetorApp;

public static class AppDefaults
{
    public static readonly SetOnce<ILoggerFactory> LoggerFactory = new();
}