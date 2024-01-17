namespace CatCryptor.WebApp.Services.HostedServices;

/// <summary>
/// Hosted service responsible for periodically cleaning up files using <see cref="FileCleanupService"/>.
/// </summary>
/// <param name="fileCleanup">The service responsible for file cleanup operations.</param>
public class FileCleanupHostedService(FileCleanupService fileCleanup) : IHostedService, IDisposable
{
    private readonly FileCleanupService _fileCleanup = fileCleanup;
    private Timer? _timer;

    /// <summary>
    /// Starts the hosted service, initiating periodic file cleanup.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer((state) => { _fileCleanup.CleanupFiles(); }, null, TimeSpan.Zero, TimeSpan.FromHours(1));
        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops the hosted service, halting further periodic file cleanup.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Disposes of the timer used for periodic file cleanup.
    /// </summary>
    public void Dispose() =>
        _timer?.Dispose();
}