using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore
{
    public interface IEventStoreClient
    {
        /// <summary>
        /// Writes a collection of event objects to an existing stream.
        /// </summary>
        /// <remarks>Call will fail, if stream is empty.</remarks>
        /// <param name="streamId">Event stream to write events too.</param>
        /// <param name="events">Collection of event objects to write.</param>
        /// <param name="version">Set this value to the latest known version of the stream to enable optimistic currency.</param>
        /// <param name="options">(Optional) The options for writing events.</param>
        /// <param name="cancellationToken">(Optional) <seealso cref="CancellationToken"/> representing request cancellation.</param>
        /// <returns>Response of the write operation.</returns>
        ValueTask<StreamResponse> WriteToStreamAsync(
            StreamId streamId,
            IReadOnlyCollection<object> events,
            StreamVersion version,
            StreamWriteOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Read events from stream.
        /// </summary>
        /// <param name="streamId">Event stream to read from.</param>
        /// <param name="fromVersion">(Optional) Start reading stream from a given version.</param>
        /// <param name="cancellationToken">(Optional) <seealso cref="CancellationToken"/> representing request cancellation.</param>
        /// <returns>List of events from stream.</returns>
        IAsyncEnumerable<IEvent> ReadFromStreamAsync(
            StreamId streamId,
            StreamVersion? fromVersion = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets current state for a specific stream.
        /// </summary>
        /// <param name="streamId">Event stream to read from.</param>
        /// <param name="cancellationToken">(Optional) <seealso cref="CancellationToken"/> representing request cancellation.</param>
        /// <returns>Stream information.</returns>
        ValueTask<IStreamMetadata> GetStreamInfoAsync(
            StreamId streamId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets or creates an event subscription for all streams.
        /// </summary>
        /// <param name="consumerGroup">The name the subscription is persisted with.</param>
        /// <param name="startOptions">Start options for subscription.</param>
        /// <param name="eventsHandler">Delegate called when events arrives.</param>
        /// <param name="errorHandler">Delegate called when an exception occurs.</param>
        /// <returns>Event subscription.</returns>
        IStreamSubscription SubscribeToStreams(
            ConsumerGroup consumerGroup,
            SubscriptionStartOptions startOptions,
            ProcessEventsHandler eventsHandler,
            ProcessExceptionHandler errorHandler);

        /// <summary>
        /// Delete subscription.
        /// </summary>
        /// <param name="consumerGroup">Consumer group to remove subscription from.</param>
        /// <param name="cancellationToken">(Optional) <seealso cref="CancellationToken"/> representing request cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        ValueTask DeleteSubscribeAsync(
            ConsumerGroup consumerGroup,
            CancellationToken cancellationToken = default);
    }
}