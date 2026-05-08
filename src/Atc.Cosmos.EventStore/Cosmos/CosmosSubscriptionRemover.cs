namespace Atc.Cosmos.EventStore.Cosmos;

internal class CosmosSubscriptionRemover : IStreamSubscriptionRemover
{
    private readonly IEventStoreContainerProvider containerProvider;

    public CosmosSubscriptionRemover(
        IEventStoreContainerProvider containerProvider)
    {
        this.containerProvider = containerProvider;
    }

    public async Task DeleteAsync(
        ConsumerGroup consumerGroup,
        CancellationToken cancellationToken)
    {
        var resultSet = containerProvider
            .GetSubscriptionContainer()
            .GetItemQueryIterator<SubscriptionLease>(
                new QueryDefinition("SELECT r.id FROM r WHERE STARTSWITH(r.id, @name, false)")
                    .WithParameter("@name", GetProcessorName(consumerGroup)));

        while (resultSet.HasMoreResults)
        {
            var registrations = await resultSet
                .ReadNextAsync(cancellationToken)
                .ConfigureAwait(false);
            foreach (var id in registrations.Select(r => r.Id))
            {
                await containerProvider
                    .GetSubscriptionContainer()
                    .DeleteItemAsync<object>(
                        id,
                        new PartitionKey(id),
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }

    private static string GetProcessorName(ConsumerGroup consumerGroup)
        => consumerGroup.Name + ":";

    internal class SubscriptionLease
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
    }
}