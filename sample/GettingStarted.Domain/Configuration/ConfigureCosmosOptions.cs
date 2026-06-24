namespace GettingStarted.Configuration;

/// <summary>
/// Fills in the local Cosmos DB Emulator defaults when no account endpoint has been
/// supplied (e.g. running standalone, outside the Aspire AppHost), and pins the database name.
/// </summary>
internal sealed class ConfigureCosmosOptions : IConfigureOptions<CosmosOptions>
{
    public void Configure(CosmosOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.AccountEndpoint))
        {
            options.AccountEndpoint = EventStoreClientOptions.EmulatorEndpoint;
            options.AccountKey = EventStoreClientOptions.EmulatorAuthKey;
        }

        options.DatabaseName = SampleConstants.DatabaseName;
    }
}