namespace GettingStarted.Configuration;

/// <summary>
/// Points the event store at the same Cosmos DB Emulator endpoint used for read models,
/// reading the endpoint injected by the Aspire AppHost (or falling back to the local default).
/// </summary>
internal sealed class ConfigureEventStoreOptions(IConfiguration configuration)
    : IConfigureOptions<EventStoreClientOptions>
{
    public void Configure(EventStoreClientOptions options)
    {
        var endpoint =
            configuration[$"{nameof(CosmosOptions)}:{nameof(CosmosOptions.AccountEndpoint)}"]
            ?? EventStoreClientOptions.EmulatorEndpoint;

        options.UseCosmosEmulator(endpoint, allowAnyServerCertificate: true);
        options.EventStoreDatabaseId = SampleConstants.DatabaseName;
    }
}