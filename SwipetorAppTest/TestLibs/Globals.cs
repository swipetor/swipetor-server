using System.Collections.Generic;
using Divergic.Logging.Xunit;
using Microsoft.Extensions.Logging;
using SwipetorApp;
using WebAppShared.Internal;
using Xunit.Abstractions;

namespace SwipetorAppTest.TestLibs;

public static class TestGlobals
{
    public static void Initialize(ITestOutputHelper outputHelper)
    {
        var loggerProvider = new TestOutputLoggerProvider(outputHelper, new LoggingConfig
        {
            LogLevel = LogLevel.Debug
        });

        var loggerFactory = new LoggerFactory(new List<ILoggerProvider> { loggerProvider });
        WebAppSharedDefaults.SetLoggerFactory(loggerFactory);
        AppDefaults.LoggerFactory.Value = loggerFactory;
    }
}