using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Atc.Cosmos.EventStore.Events;
using Atc.Cosmos.EventStore.Streams;
using Microsoft.Azure.Cosmos;

namespace Atc.Cosmos.EventStore.Cosmos
{
    public class CosmosStreamIterator : IStreamIterator
    {
        private readonly IEventStoreContainerProvider containerProvider;

        public CosmosStreamIterator(IEventStoreContainerProvider containerProvider)
        {
            this.containerProvider = containerProvider;
        }

        public async IAsyncEnumerable<IEvent> ReadAsync(
            StreamId streamId,
            StreamVersion fromVersion,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var pk = new PartitionKey(streamId.Value);
            var resultSet = containerProvider
                .GetStreamContainer()
                .GetItemQueryIterator<EventDocument>(
                    GetQuery(streamId, fromVersion),
                    requestOptions: GetRequestOptions(pk));

            while (resultSet.HasMoreResults && !cancellationToken.IsCancellationRequested)
            {
                var items = await resultSet
                    .ReadNextAsync(cancellationToken)
                    .ConfigureAwait(false);

                foreach (var item in items.Resource)
                {
                    yield return item;
                }
            }
        }

        private static QueryRequestOptions GetRequestOptions(PartitionKey partitionKey)
            => new() { PartitionKey = partitionKey, };

        private static QueryDefinition GetQuery(StreamId streamId, StreamVersion fromVersion)
            => fromVersion == StreamVersion.Any || fromVersion == StreamVersion.NotEmpty
             ? GetAllVersionsQuery((string)streamId)
             : GetFromVersionQuery((string)streamId, (long)fromVersion);

        private static QueryDefinition GetAllVersionsQuery(string partitionKey)
            => new QueryDefinition(
                "SELECT * FROM events e WHERE e.pk = @partitionKey ORDER BY e.properties.version")
                .WithParameter("@partitionKey", partitionKey);

        private static QueryDefinition GetFromVersionQuery(string partitionKey, long fromVersion)
            => new QueryDefinition(
                "SELECT * FROM events e WHERE e.pk = @partitionKey AND e.properties.version >= @fromVersion ORDER BY e.properties.version")
                .WithParameter("@partitionKey", partitionKey)
                .WithParameter("@fromVersion", fromVersion);
    }
}