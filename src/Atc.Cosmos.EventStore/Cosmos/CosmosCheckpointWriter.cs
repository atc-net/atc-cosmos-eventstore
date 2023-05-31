using Atc.Cosmos.EventStore.Events;
using Atc.Cosmos.EventStore.Streams;
using Microsoft.Azure.Cosmos;

namespace Atc.Cosmos.EventStore.Cosmos;

internal class CosmosCheckpointWriter : IStreamCheckpointWriter
{
    private readonly IEventStoreContainerProvider containerProvider;
    private readonly IDateTimeProvider dateTimeProvider;

    public CosmosCheckpointWriter(
        IEventStoreContainerProvider containerProvider,
        IDateTimeProvider dateTimeProvider)
    {
        this.containerProvider = containerProvider;
        this.dateTimeProvider = dateTimeProvider;
    }

    public Task WriteAsync(
        string name,
        StreamId streamId,
        StreamVersion streamVersion,
        object? state,
        CancellationToken cancellationToken)
        => containerProvider
            .GetIndexContainer()
            .UpsertItemAsync(
                new CheckpointDocument(
                    name,
                    streamId,
                    streamVersion,
                    dateTimeProvider.GetDateTime(),
                    state),
                new PartitionKey(streamId.Value),
                new ItemRequestOptions { EnableContentResponseOnWrite = false },
                cancellationToken);
}