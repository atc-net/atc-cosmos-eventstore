using Microsoft.Azure.Cosmos;

namespace Atc.Cosmos.EventStore.Cosmos
{
    public interface ICosmosClientFactory
    {
        CosmosClient GetClient();
    }
}