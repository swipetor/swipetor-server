using Microsoft.Extensions.Logging;
using WebLibServer.Types;

namespace SwipetorApp;

public static class AppDefaults
{
    public static readonly SetOnce<ILoggerFactory> LoggerFactory = new();
}