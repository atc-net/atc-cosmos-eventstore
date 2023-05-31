using System.Runtime.CompilerServices;
using Atc.Cosmos.EventStore.Streams;
using Microsoft.Azure.Cosmos;

namespace Atc.Cosmos.EventStore.Cosmos;

internal class CosmosStreamIndexReader : IStreamIndexReader
{
    private readonly IEventStoreContainerProvider containerProvider;

    public CosmosStreamIndexReader(
        IEventStoreContainerProvider containerProvider)
        => this.containerProvider = containerProvider;

    public async IAsyncEnumerable<IStreamIndex> ReadAsync(
        string? filter,
        DateTimeOffset? createdAfter,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var pk = new PartitionKey(nameof(StreamIndex));
        var resultSet = containerProvider
            .GetIndexContainer()
            .GetItemQueryIterator<StreamIndex>(
                GetQuery(filter, createdAfter),
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

    private static QueryDefinition GetQuery(
        string? filter,
        DateTimeOffset? createdAfter)
        => filter is null
         ? GetAllQuery(createdAfter)
         : GetFilterQuery(filter, createdAfter);

    private static QueryDefinition GetAllQuery(DateTimeOffset? createdAfter)
        => createdAfter is not null
         ? new QueryDefinition(
             "SELECT * FROM c WHERE c.timestamp > @createdAfter")
             .WithParameter("@createdAfter", createdAfter)
         : new QueryDefinition("SELECT * FROM c");

    private static QueryDefinition GetFilterQuery(string filter, DateTimeOffset? createdAfter)
        => createdAfter is not null
         ? new QueryDefinition(
            "SELECT * FROM c WHERE c.id LIKE @filter AND c.timestamp > @createdAfter")
            .WithParameter("@filter", filter)
            .WithParameter("@createdAfter", createdAfter)
         : new QueryDefinition(
            "SELECT * FROM c WHERE c.id LIKE @filter")
            .WithParameter("@filter", filter);
}