using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SwipetorApp.System;

/// <summary>
///     Kills the application immediately when ctrl+c is pressed.
/// </summary>
/// <param name="logger"></param>
public class NoopConsoleLifetime(ILogger<NoopConsoleLifetime> logger) : IHostLifetime, IDisposable
{
    public void Dispose()
    {
        Console.CancelKeyPress -= OnCancelKeyPressed;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("StopAsync was called");
        return Task.CompletedTask;
    }

    public Task WaitForStartAsync(CancellationToken cancellationToken)
    {
        // without this ctrl+c just kills the application
        Console.CancelKeyPress += OnCancelKeyPressed;
        return Task.CompletedTask;
    }

    private void OnCancelKeyPressed(object? sender, ConsoleCancelEventArgs e)
    {
        logger.LogInformation("Ctrl+C has been pressed, ignoring.");
        // e.Cancel = false would turn off the application without the clean up
        e.Cancel = false;
        // not running in task causes dead lock
        // Environment.Exit calls the ProcessExit event which waits for web server which waits for this method to end

        Task.Run(() => Environment.Exit(1));
    }
}