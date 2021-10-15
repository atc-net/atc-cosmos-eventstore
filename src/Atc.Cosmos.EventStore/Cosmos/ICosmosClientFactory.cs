using Microsoft.Azure.Cosmos;

namespace Atc.Cosmos.EventStore.Cosmos
{
    internal interface ICosmosClientFactory
    {
        CosmosClient GetClient();
    }
}