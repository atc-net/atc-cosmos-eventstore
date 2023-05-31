namespace Atc.Cosmos.EventStore;

/// <summary>
/// Delegate for handling exceptions in a subscription.
/// </summary>
/// <param name="leaseToken">A unique identifier for the lease.</param>
/// <param name="exception">The exception received.</param>
/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
public delegate Task ProcessExceptionHandler(
    string leaseToken,
    Exception exception);