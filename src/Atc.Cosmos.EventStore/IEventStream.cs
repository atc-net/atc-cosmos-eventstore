using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BigBang.Cosmos.EventStore
{
    public delegate Task SubscriptionDelegate(IReadOnlyCollection<Event> events, CancellationToken cancellationToken);

    public interface IEventStream
    {
        /// <summary>
        /// Writes a collection of event objects to a stream.
        /// </summary>
        /// <param name="streamId">Identity of the stream.</param>
        /// <param name="events">Collection of event objects.</param>
        /// <param name="etag">Expected .</param>
        /// <param name="correlationId">Optional correlation id store as part of the EventProperties.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Response of the write operation.</returns>
        Task<EventStreamResponse> WriteEventsAsync(
            string streamId,
            IReadOnlyCollection<object> events,
            string? etag = null,
            string? correlationId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Continuously fetching events from the stream across all instances/id's when they become available.
        /// </summary>
        /// <remarks>
        ///   Progress is tracked by <paramref name="name"/> and persisted when a call is completed.
        ///   This enables scenarios.
        /// </remarks>
        /// <param name="name">Name of the.</param>
        /// <param name="instanceName">.</param>
        /// <param name="pollingInterval">A.</param>
        /// <param name="cancellationToken">The enumerable will never end until the cancellation token is canceled.</param>
        /// <returns>List of events received.</returns>
        IAsyncEnumerable<IReadOnlyCollection<Event>> CountinuousReadStreamAsync(
            string name,
            string? instanceName = null,
            TimeSpan? pollingInterval = default,
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<IReadOnlyCollection<Event>> ReadStreamAsync(
            string streamId,
            string? fromEtag = null,
            CancellationToken cancellationToken = default);

        EventStreamSubscription CreateOrResumeSubscription(
            string name,
            SubscriptionDelegate callback,
            string? instanceName = null,
            TimeSpan? pollingInterval = null);
    }
}