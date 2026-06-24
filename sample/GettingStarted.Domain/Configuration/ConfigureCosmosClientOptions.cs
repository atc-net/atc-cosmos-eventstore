namespace GettingStarted.Configuration;

/// <summary>
/// Configures the underlying <see cref="CosmosClientOptions"/> for talking to the
/// Cosmos DB Emulator, whose self-signed certificate is not trusted by default.
/// Atc.Cosmos copies these settings onto the client it builds.
/// </summary>
internal sealed class ConfigureCosmosClientOptions : IConfigureOptions<CosmosClientOptions>
{
    public void Configure(CosmosClientOptions options)
    {
        options.ConnectionMode = ConnectionMode.Gateway;
        options.LimitToEndpoint = true;

        // Accept the emulator's self-signed certificate. Safe here because the sample
        // only ever connects to a local emulator — never do this against a real account.
#pragma warning disable CA5400, MA0039, S4830
        options.HttpClientFactory = () => new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
        });
#pragma warning restore CA5400, MA0039, S4830
    }
}