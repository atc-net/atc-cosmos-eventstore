using System;
using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore
{
    /// <summary>
    /// Delegate for handling exceptions in a subscription.
    /// </summary>
    /// <param name="exception">The exception received.</param>
    /// <param name="cancellationToken"><seealso cref="CancellationToken"/> representing request cancellation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public delegate ValueTask ProcessExceptionHandler(
        Exception exception,
        CancellationToken cancellationToken);
}
