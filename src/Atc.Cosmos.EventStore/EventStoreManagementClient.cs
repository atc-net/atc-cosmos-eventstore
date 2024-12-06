using Atc.Cosmos.EventStore.Streams;

namespace Atc.Cosmos.EventStore;

internal class EventStoreManagementClient : IEventStoreManagementClient
{
    private readonly IStreamDeleter streamDeleter;

    public EventStoreManagementClient(IStreamDeleter streamDeleter)
    {
        this.streamDeleter = streamDeleter;
    }

    public Task DeleteStreamAsync(
        StreamId streamId,
        CancellationToken cancellationToken = default)
        => streamDeleter.DeleteAsync(streamId, cancellationToken);

    public Task PurgeStreamAsync(
        StreamId streamId,
        StreamVersion version,
        long count,
        CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<StreamResponse> RetireStreamAsync(
        StreamId streamId,
        StreamVersion? expectedVersion = default,
        CancellationToken cancellationToken = default)
        => throw new System.NotImplementedException();
}