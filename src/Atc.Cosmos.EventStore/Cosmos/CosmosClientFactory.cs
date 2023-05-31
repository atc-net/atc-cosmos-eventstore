using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Atc.Cosmos.EventStore.Cosmos;

internal sealed class CosmosClientFactory : ICosmosClientFactory, IDisposable
{
    private readonly CosmosClient cosmosClient;
    private bool disposedValue;

    [SuppressMessage(
        "Critical Vulnerability",
        "S4830:Server certificates should be verified during SSL/TLS connections",
        Justification = "This is only allowed when running against cosmos emulator")]
    public CosmosClientFactory(
        IOptions<EventStoreClientOptions> options,
        CosmosEventSerializer eventSerializer)
    {
        options.Value.CosmosClientOptions.Serializer = eventSerializer;

        if (options.Value.AllowAnyServerCertificate)
        {
            options.Value.CosmosClientOptions.HttpClientFactory = ()
                => new HttpClient(
                    new HttpClientHandler()
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                        CheckCertificateRevocationList = true,
                    });
            options.Value.CosmosClientOptions.ConnectionMode = ConnectionMode.Gateway;
        }

        cosmosClient = options.Value.Credential is null
            ? new CosmosClient(
                options.Value.Endpoint,
                options.Value.AuthKey,
                options.Value.CosmosClientOptions)
            : new CosmosClient(
                options.Value.Endpoint,
                options.Value.Credential,
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