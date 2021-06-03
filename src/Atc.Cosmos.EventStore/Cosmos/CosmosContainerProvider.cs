using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Atc.Cosmos.EventStore.Cosmos
{
    public class CosmosContainerProvider : IEventStoreContainerProvider
    {
        private readonly ICosmosClientFactory factory;
        private readonly string databaseId;
        private readonly string containerId;
        private readonly string subscriptionId;

        public CosmosContainerProvider(
            ICosmosClientFactory cosmosClientFactory,
            IOptions<EventStoreClientOptions> options)
        {
            factory = cosmosClientFactory;
            databaseId = options.Value.EventStoreDatabaseId;
            containerId = options.Value.EventStoreContainerId;
            subscriptionId = options.Value.SubscriptionContainerId;
        }

        public Container GetStreamContainer()
            => factory
                .GetClient()
                .GetContainer(
                    databaseId,
                    containerId);

        public Container GetSubscriptionContainer()
            => factory
                .GetClient()
                .GetContainer(
                    databaseId,
                    subscriptionId);
    }
}