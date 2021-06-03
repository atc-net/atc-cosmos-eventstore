using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Atc.Cosmos.EventStore.DependencyInjection.Internal
{
    public class EventStoreCosmosBuilder : IEventStoreCosmosBuilder
    {
        private readonly EventStoreClientOptions options;

        public EventStoreCosmosBuilder(IOptions<EventStoreClientOptions> options)
        {
            this.options = options.Value;
        }

        public IEventStoreCosmosBuilder UseCosmosClientOptions(CosmosClientOptions cosmosOptions)
        {
            options.CosmosClientOptions = cosmosOptions;

            return this;
        }

        public IEventStoreCosmosBuilder UseDatabase(string databaseId)
        {
            options.EventStoreDatabaseId = databaseId;

            return this;
        }

        public IEventStoreCosmosBuilder UseStreamContainer(string containerId)
        {
            options.EventStoreContainerId = containerId;

            return this;
        }

        public IEventStoreCosmosBuilder UseSubscriptionContainer(string containerId)
        {
            options.SubscriptionContainerId = containerId;

            return this;
        }

        public EventStoreClientOptions Build()
            => options;
    }
}