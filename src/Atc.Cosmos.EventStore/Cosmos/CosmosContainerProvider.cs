using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Atc.Cosmos.EventStore.Cosmos
{
    internal class CosmosContainerProvider : IEventStoreContainerProvider
    {
        private readonly ICosmosClientFactory factory;
        private readonly string databaseId;
        private readonly string containerId;
        private readonly string indexId;
        private readonly string subscriptionId;

        public CosmosContainerProvider(
            ICosmosClientFactory cosmosClientFactory,
            IOptions<EventStoreClientOptions> options)
        {
            factory = cosmosClientFactory;
            databaseId = options.Value.EventStoreDatabaseId;
            containerId = options.Value.EventStoreContainerId;
            indexId = options.Value.IndexContainerId;
            subscriptionId = options.Value.SubscriptionContainerId;
        }

        public Container GetStreamContainer()
            => factory
                .GetClient()
                .GetContainer(
                    databaseId,
                    containerId);

        public Container GetIndexContainer()
            => factory
                .GetClient()
                .GetContainer(
                    databaseId,
                    indexId);

        public Container GetSubscriptionContainer()
            => factory
                .GetClient()
                .GetContainer(
                    databaseId,
                    subscriptionId);
    }
}