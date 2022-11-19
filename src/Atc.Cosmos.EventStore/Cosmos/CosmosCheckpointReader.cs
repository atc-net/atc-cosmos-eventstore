using System.Net;
using Atc.Cosmos.EventStore.Streams;
using Microsoft.Azure.Cosmos;

namespace Atc.Cosmos.EventStore.Cosmos;

internal class CosmosCheckpointReader : IStreamCheckpointReader
{
    private readonly IEventStoreContainerProvider containerProvider;

    public CosmosCheckpointReader(
        IEventStoreContainerProvider containerProvider)
        => this.containerProvider = containerProvider;

    public async Task<Checkpoint<TState>?> ReadAsync<TState>(
        string name,
        StreamId streamId,
        CancellationToken cancellationToken)
    {
        try
        {
            var checkpoint = await containerProvider
                .GetIndexContainer()
                .ReadItemAsync<CheckpointDocument<TState>>(
                    name,
                    new PartitionKey(streamId.Value),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return new Checkpoint<TState>(
                checkpoint.Resource.Name,
                checkpoint.Resource.StreamId,
                checkpoint.Resource.StreamVersion,
                checkpoint.Resource.Timestamp,
                checkpoint.Resource.State);
        }
        catch (CosmosException ex)
        when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }
}