using Microsoft.Azure.Cosmos;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface IEventStoreCosmosBuilder
    {
        IEventStoreCosmosBuilder UseCosmosClientOptions(CosmosClientOptions cosmosOptions);

        IEventStoreCosmosBuilder UseDatabase(string databaseId);

        IEventStoreCosmosBuilder UseStreamContainer(string containerId);

        IEventStoreCosmosBuilder UseSubscriptionContainer(string containerId);
    }
}