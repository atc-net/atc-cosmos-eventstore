using System.Runtime.CompilerServices;
using Atc.Cosmos.EventStore.Cosmos;
using Atc.Cosmos.EventStore.Streams;
using Microsoft.Azure.Cosmos;
using StreamState = Atc.Cosmos.EventStore.StreamState;

namespace Atc.Cosmos.EventStore.Cqrs.Tests;

#nullable enable

/// <summary>
/// A working in-memory implementation of Atc.Cosmos.EventStore.IEventStoreClient.
/// </summary>
public sealed class InMemoryEventStoreClient(IStreamReadValidator readValidator) : IEventStoreClient
{
    private readonly Dictionary<StreamId, InMemoryStream> streams = new();
    private readonly List<InMemoryStreamSubscription> subscriptions = new();

    /// <inheritdoc />
    public async Task<StreamResponse> WriteToStreamAsync(
        StreamId streamId,
        IReadOnlyCollection<object> events,
        StreamVersion? version = null,
        StreamWriteOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        // Get stream
        if (!streams.TryGetValue(streamId, out var stream))
        {
            stream = new InMemoryStream(streamId);
            streams.Add(streamId, stream);
        }

        // Add events to stream
        var writtenEvents = stream.AddEvents(events);

        // Invoke subscriptions
        // Note that this way of invoking the subscriptions (used by projections jobs) means that projections runs as part of
        // this method (WriteToStreamAsync). A nice benefit from this implementation is that we can assert most side-effects from a command
        // execution, eg projections being written, directly after command has been executed.
        // When using a real Cosmos DB projections runs some time *after* writing of events to Cosmos.
        foreach (var sub in subscriptions)
        {
            try
            {
                await sub.EventsHandler(writtenEvents, CancellationToken.None);
            }
            catch (Exception ex)
            {
                await sub.ExceptionHandler("leasetoken", ex);
            }
        }

        var metadata = stream.Metadata;
        return new StreamResponse(streamId, metadata.Version, metadata.Timestamp, metadata.State);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<IEvent> ReadFromStreamAsync(
        StreamId streamId,
        StreamVersion? fromVersion = null,
        StreamReadFilter? filter = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (filter is not null)
        {
            throw new NotImplementedException("Use of StreamReadFilter is not implemented");
        }

        if (!streams.TryGetValue(streamId, out var stream))
        {
            stream = new InMemoryStream(streamId);
        }

        // Validate like the real implementation
        readValidator.Validate(stream.Metadata, fromVersion ?? StreamVersion.Any);

        // StreamVersion is modelled as EventStreamVersion in CQS package
        // StreamEmpty = 0
        // Exists = -1
        // Any = 9223372036854775807L
        if (fromVersion.HasValue && !fromVersion.Value.Equals(StreamVersion.Any) &&
            fromVersion.Value > StreamVersion.ToStreamVersion(0))
        {
            throw new NotImplementedException(
                $"Use of StreamVersion other than EventStreamVersion.Any, EventStreamVersion.Exists, EventStreamVersion.StreamEmpty is not implemented. Version {fromVersion.Value.Value} was provided");
        }

        await Task.Yield(); // Force async
        foreach (var @event in stream.Events)
        {
            yield return @event;
        }
    }

    /// <inheritdoc />
    public Task<IStreamMetadata> GetStreamInfoAsync(
        StreamId streamId,
        CancellationToken cancellationToken = default)
    {
        if (streams.TryGetValue(streamId, out var stream))
        {
            return Task.FromResult<IStreamMetadata>(stream.Metadata);
        }

        return Task.FromResult<IStreamMetadata>(new InMemoryStreamMetadata(
            "ETag",
            StreamState.New,
            streamId,
            DateTimeOffset.UtcNow,
            StreamVersion.ToStreamVersion(0)));
    }

    /// <inheritdoc />
    public IStreamSubscription SubscribeToStreams(
        ConsumerGroup consumerGroup,
        ProcessEventsHandler eventsHandler,
        ProcessExceptionHandler exceptionHandler)
    {
        var subscription = new InMemoryStreamSubscription(eventsHandler, exceptionHandler);
        this.subscriptions.Add(subscription);
        return subscription;
    }

    /// <inheritdoc />
    public Task DeleteSubscriptionAsync(ConsumerGroup consumerGroup, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    /// <inheritdoc />
    public IAsyncEnumerable<IStreamIndex> QueryStreamsAsync(
        string? filter = null,
        DateTimeOffset? createdAfter = null,
        CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task SetStreamCheckpointAsync(
        string name,
        StreamId streamId,
        StreamVersion version,
        object? state = null,
        CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Checkpoint<T>?> GetStreamCheckpointAsync<T>(
        string name,
        StreamId streamId,
        CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Checkpoint?> GetStreamCheckpointAsync(
        string name,
        StreamId streamId,
        CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task DeleteStreamAsync(StreamId streamId, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    private sealed record InMemoryStreamSubscription(
        ProcessEventsHandler EventsHandler,
        ProcessExceptionHandler ExceptionHandler)
        : IStreamSubscription
    {
        public Task StartAsync() => Task.CompletedTask;

        public Task StopAsync() => Task.CompletedTask;
    }

    private sealed class InMemoryStream(StreamId streamId)
    {
        private int currentVersion;

        public List<InMemoryEvent> Events { get; } = new();

        public InMemoryStreamMetadata Metadata => new InMemoryStreamMetadata(
            "ETag",
            Events.Count == 0 ? StreamState.New : StreamState.Active,
            streamId,
            DateTimeOffset.UtcNow,
            StreamVersion.ToStreamVersion(currentVersion));

        public IReadOnlyList<InMemoryEvent> AddEvents(IEnumerable<object> events)
        {
            var wrappedEvents = events.Select(e => new InMemoryEvent
            {
                Data = e,
                Metadata = new InMemoryEventMetadata
                {
                    Name = e.GetType().Name,
                    StreamId = streamId,
                    Version = ++currentVersion,
                    Timestamp = DateTimeOffset.UtcNow,
                },
            }).ToList();

            Events.AddRange(wrappedEvents);
            return wrappedEvents;
        }
    }

    private sealed class InMemoryEvent : IEvent
    {
        required public object Data { get; init; }

        required public IEventMetadata Metadata { get; init; }
    }

    private sealed class InMemoryEventMetadata : IEventMetadata
    {
        required public string Name { get; init; }

        public string? CorrelationId { get; init; }

        public string? CausationId { get; init; }

        required public StreamId StreamId { get; init; }

        public DateTimeOffset Timestamp { get; init; }

        required public StreamVersion Version { get; init; }
    }

    private sealed record InMemoryStreamMetadata(
        string ETag,
        StreamState State,
        StreamId StreamId,
        DateTimeOffset Timestamp,
        StreamVersion Version) : IStreamMetadata;
}

/// <summary>
/// No op implementation of <see cref="IEventStoreInitializer"/>.
/// </summary>
public sealed class NoOpEventStoreInitializer : IEventStoreInitializer
{
    /// <inheritdoc />
    public Task CreateEventStoreAsync(ThroughputProperties throughputProperties, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void CreateEventStore(ThroughputProperties throughputProperties)
    {
        // No op
    }
}