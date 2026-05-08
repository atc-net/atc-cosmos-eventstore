namespace Atc.Cosmos.EventStore.Cosmos;

internal class CosmosDeleter : IStreamDeleter
{
    private readonly IEventStoreContainerProvider containerProvider;

    public CosmosDeleter(IEventStoreContainerProvider containerProvider)
    {
        this.containerProvider = containerProvider;
    }

    public async Task DeleteAsync(
        StreamId streamId,
        CancellationToken cancellationToken)
    {
        var pk = new PartitionKey(streamId.Value);
        var container = containerProvider.GetStreamContainer();
        var response = await container.DeleteAllItemsByPartitionKeyStreamAsync(pk, null, cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}