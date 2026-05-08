namespace Atc.Cosmos.EventStore.Cosmos;

internal interface ICosmosClientFactory
{
    CosmosClient GetClient();
}