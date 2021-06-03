using System;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Atc.Cosmos.EventStore.Cosmos
{
    public sealed class CosmosClientFactory : ICosmosClientFactory, IDisposable
    {
        private readonly CosmosClient cosmosClient;
        private bool disposedValue;

        public CosmosClientFactory(
            IOptions<EventStoreClientOptions> options,
            CosmosEventSerializer eventSerializer)
        {
            options.Value.CosmosClientOptions.Serializer = eventSerializer;
            cosmosClient = new CosmosClient(
                options.Value.ConnectionString,
                options.Value.CosmosClientOptions);
        }

        public CosmosClient GetClient()
            => cosmosClient;

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    cosmosClient.Dispose();
                }

                disposedValue = true;
            }
        }
    }
}