using System.Runtime.CompilerServices;
using Atc.Cosmos.EventStore.Events;
using Atc.Cosmos.EventStore.Streams;
using Microsoft.Azure.Cosmos;

namespace Atc.Cosmos.EventStore.Cosmos;

internal class CosmosStreamIterator : IStreamIterator
{
    private readonly IEventStoreContainerProvider containerProvider;

    public CosmosStreamIterator(
        IEventStoreContainerProvider containerProvider)
        => this.containerProvider = containerProvider;

    public async IAsyncEnumerable<IEvent> ReadAsync(
        StreamId streamId,
        StreamVersion fromVersion,
        StreamReadFilter? filter,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var pk = new PartitionKey(streamId.Value);
        var resultSet = containerProvider
            .GetStreamContainer()
            .GetItemQueryIterator<EventDocument>(
                CosmosStreamQueryBuilder.GetQueryDefinition(streamId, fromVersion, filter),
                requestOptions: new() { PartitionKey = pk });

        while (resultSet.HasMoreResults && !cancellationToken.IsCancellationRequested)
        {
            var items = await resultSet
                .ReadNextAsync(cancellationToken)
                .ConfigureAwait(false);

            foreach (var item in items.Resource.Where(d => d is not null))
            {
                yield return item;
            }
        }
    }
}