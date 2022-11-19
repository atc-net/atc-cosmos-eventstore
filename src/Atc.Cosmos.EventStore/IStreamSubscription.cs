namespace Atc.Cosmos.EventStore;

public interface IStreamSubscription
{
    /// <summary>
    /// Start processing subscription.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task StartAsync();

    /// <summary>
    /// Stop processing.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task StopAsync();
}