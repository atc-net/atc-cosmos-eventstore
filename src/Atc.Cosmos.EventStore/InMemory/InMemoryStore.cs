using System.Collections.Concurrent;
using Atc.Cosmos.EventStore.Cosmos;
using Atc.Cosmos.EventStore.Events;
using Atc.Cosmos.EventStore.Streams;

namespace Atc.Cosmos.EventStore.InMemory;

internal class InMemoryStore :
    IStreamMetadataReader,
    IStreamIterator,
    IStreamBatchWriter,
    IStreamSubscriptionFactory,
    IStreamSubscriptionRemover,
    IStreamIndexReader,
    IStreamCheckpointReader,
    IStreamCheckpointWriter
{
    private readonly IDateTimeProvider dateTimeProvider;

    public InMemoryStore(
        IDateTimeProvider dateTimeProvider)
    {
        this.dateTimeProvider = dateTimeProvider;
    }

    public ConcurrentDictionary<StreamId, List<IEvent>> EventStore { get; }
        = new ConcurrentDictionary<StreamId, List<IEvent>>();

    public ConcurrentDictionary<StreamId, ConcurrentDictionary<string, CheckpointDocument>> Checkpoints { get; }
        = new ConcurrentDictionary<StreamId, ConcurrentDictionary<string, CheckpointDocument>>();

    IStreamSubscription IStreamSubscriptionFactory.Create(
        ConsumerGroup consumerGroup,
        ProcessEventsHandler eventsHandler,
        ProcessExceptionHandler exceptionHandler)
        => throw new NotImplementedException();

    Task IStreamSubscriptionRemover.DeleteAsync(
        ConsumerGroup consumerGroup,
        CancellationToken cancellationToken)
        => throw new NotImplementedException();

    Task<IStreamMetadata> IStreamMetadataReader.GetAsync(
        StreamId streamId,
        CancellationToken cancellationToken)
        => throw new NotImplementedException();

    IAsyncEnumerable<IEvent> IStreamIterator.ReadAsync(
        StreamId streamId,
        StreamVersion fromVersion,
        StreamReadFilter? filter,
        CancellationToken cancellationToken)
        => throw new NotImplementedException();

    IAsyncEnumerable<IStreamIndex> IStreamIndexReader.ReadAsync(
        string? filter,
        DateTimeOffset? createdAfter,
        CancellationToken cancellationToken)
        => throw new NotImplementedException();

    Task<Checkpoint<TState>?> IStreamCheckpointReader.ReadAsync<TState>(
        string name,
        StreamId streamId,
        CancellationToken cancellationToken)
        => throw new NotImplementedException();

    Task<IStreamMetadata> IStreamBatchWriter.WriteAsync(
        StreamBatch batch,
        CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public Task WriteAsync(
        string name,
        StreamId streamId,
        StreamVersion streamVersion,
        object? state,
        CancellationToken cancellationToken)
    {
        Checkpoints
            .GetOrAdd(streamId, new ConcurrentDictionary<string, CheckpointDocument>())
            .AddOrUpdate(
                name,
                key => new CheckpointDocument(name, streamId, streamVersion, dateTimeProvider.GetDateTime(), state),
                (key, doc) => new CheckpointDocument(name, streamId, streamVersion, dateTimeProvider.GetDateTime(), state));

        return Task.CompletedTask;
    }
}