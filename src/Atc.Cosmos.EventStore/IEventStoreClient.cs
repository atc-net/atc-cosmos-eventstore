using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore
{
    public interface IEventStoreClient
    {
        /// <summary>
        /// Writes a collection of event objects to a stream.
        /// </summary>
        /// <param name="streamId">Event stream to write events too.</param>
        /// <param name="events">Collection of event objects to write.</param>
        /// <param name="version">
        ///   (Optional) Set the required version of the stream.
        ///   Set this value to the latest known version of the stream to enable optimistic currency.
        ///   When the stream is required to be empty use <see cref="StreamVersion.StartOfStream"/>.
        ///   Use <see cref="StreamVersion.NotEmpty"/> when the stream must contain one or more events.
        ///   To append to the end of the stream optionally specify <see cref="StreamVersion.Any"/>.
        /// </param>
        /// <param name="options">(Optional) The options for writing events.</param>
        /// <param name="cancellationToken">(Optional) <seealso cref="CancellationToken"/> representing request cancellation.</param>
        /// <returns>Response of the write operation.</returns>
        Task<StreamResponse> WriteToStreamAsync(
            StreamId streamId,
            IReadOnlyCollection<object> events,
            StreamVersion? version = default,
            StreamWriteOptions? options = default,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Read events from stream.
        /// </summary>
        /// <param name="streamId">Event stream to read from.</param>
        /// <param name="fromVersion">(Optional) Start reading stream from a given version.</param>
        /// <param name="filter">(Optional) Specify a filter to only include certain events, and/or ensure stream is at a given version.</param>
        /// <param name="cancellationToken">(Optional) <seealso cref="CancellationToken"/> representing request cancellation.</param>
        /// <returns>List of events from stream.</returns>
        IAsyncEnumerable<IEvent> ReadFromStreamAsync(
            StreamId streamId,
            StreamVersion? fromVersion = default,
            StreamReadFilter? filter = default,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets current state for a specific stream.
        /// </summary>
        /// <param name="streamId">Event stream to read from.</param>
        /// <param name="cancellationToken">(Optional) <seealso cref="CancellationToken"/> representing request cancellation.</param>
        /// <returns>Stream information.</returns>
        Task<IStreamMetadata> GetStreamInfoAsync(
            StreamId streamId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets or creates an event subscription for all streams.
        /// </summary>
        /// <param name="consumerGroup">The name the subscription is persisted with.</param>
        /// <param name="startOptions">Start options for subscription.</param>
        /// <param name="eventsHandler">Delegate called when events arrives.</param>
        /// <param name="exceptionHandler">Delegate called when an exception occurred.</param>
        /// <returns>Event subscription.</returns>
        IStreamSubscription SubscribeToStreams(
            ConsumerGroup consumerGroup,
            SubscriptionStartOptions startOptions,
            ProcessEventsHandler eventsHandler,
            ProcessExceptionHandler exceptionHandler);

        /// <summary>
        /// Delete subscription.
        /// </summary>
        /// <param name="consumerGroup">Consumer group to remove subscription from.</param>
        /// <param name="cancellationToken">(Optional) <seealso cref="CancellationToken"/> representing request cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task DeleteSubscriptionAsync(
            ConsumerGroup consumerGroup,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Search for streams matching a given filter expression.
        /// </summary>
        /// <param name="filter">
        ///   Filter expression for finding desired streams.
        ///   <seealso href="https://devblogs.microsoft.com/cosmosdb/like-keyword-cosmosdb/"/>
        /// </param>
        /// <param name="createdAfter">(Optional) exclude streams created prior to this timestamp.</param>
        /// <param name="cancellationToken">(Optional) <seealso cref="CancellationToken"/> representing request cancellation.</param>
        /// <returns>List of stream id found.</returns>
        IAsyncEnumerable<IStreamIndex> QueryStreamsAsync(
            string? filter = default,
            DateTimeOffset? createdAfter = default,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets a named checkpoint at a given version in the stream.
        /// </summary>
        /// <remarks>
        ///   Only one checkpoint per name can exists at any given time.
        ///   A checkpoint will be overridden when using an existing name.
        /// </remarks>
        /// <param name="name">Name of checkpoint.</param>
        /// <param name="streamId">Id of stream.</param>
        /// <param name="version">Version within the stream this checkpoint is related too.</param>
        /// <param name="state">(Optional) State object to store along side the checkpoint.</param>
        /// <param name="cancellationToken">(Optional) <seealso cref="CancellationToken"/> representing request cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetStreamCheckpointAsync(
            string name,
            StreamId streamId,
            StreamVersion version,
            object? state = default,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a named checkpoint with state from a stream.
        /// </summary>
        /// <typeparam name="T">Type of state.</typeparam>
        /// <param name="name">Name of checkpoint.</param>
        /// <param name="streamId">Id of stream.</param>
        /// <param name="cancellationToken">(Optional) <seealso cref="CancellationToken"/> representing request cancellation.</param>
        /// <returns>A statefull <see cref="Checkpoint{TState}"/> or null if not found.</returns>
        Task<Checkpoint<T>?> GetStreamCheckpointAsync<T>(
            string name,
            StreamId streamId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a named checkpoint from a stream.
        /// </summary>
        /// <param name="name">Name of checkpoint.</param>
        /// <param name="streamId">Id of stream.</param>
        /// <param name="cancellationToken">(Optional) <seealso cref="CancellationToken"/> representing request cancellation.</param>
        /// <returns>A <see cref="Checkpoint"/> or null if not found.</returns>
        Task<Checkpoint?> GetStreamCheckpointAsync(
            string name,
            StreamId streamId,
            CancellationToken cancellationToken = default);
    }
}