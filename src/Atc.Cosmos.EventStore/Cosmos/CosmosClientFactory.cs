using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Atc.Cosmos.EventStore.Cosmos;

internal sealed class CosmosClientFactory : ICosmosClientFactory, IDisposable
{
    private readonly CosmosClient cosmosClient;
    private bool disposedValue;

    public CosmosClientFactory(
        IOptions<EventStoreClientOptions> options,
        CosmosEventSerializer eventSerializer)
    {
        options.Value.CosmosClientOptions.Serializer = eventSerializer;
#pragma warning disable CS0618 // Type or member is obsolete
        cosmosClient = options.Value.Credential is null
            ? options.Value.ConnectionString is not null
                ? new CosmosClient(
                    options.Value.ConnectionString,
                    options.Value.CosmosClientOptions)
                : new CosmosClient(
                    options.Value.Endpoint,
                    options.Value.AuthKey,
                    options.Value.CosmosClientOptions)
            : new CosmosClient(
                options.Value.Endpoint,
                options.Value.Credential,
                options.Value.CosmosClientOptions);
#pragma warning restore CS0618 // Type or member is obsolete
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