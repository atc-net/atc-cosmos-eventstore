using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore
{
    /// <summary>
    /// Delegate for processing events from a subscription.
    /// </summary>
    /// <param name="events">List of events to process.</param>
    /// <param name="cancellationToken"><seealso cref="CancellationToken"/> representing request cancellation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public delegate ValueTask ProcessEventsHandler(
        IEnumerable<IEvent> events,
        CancellationToken cancellationToken);
}