using Atc.Cosmos;
using Microsoft.Extensions.Options;

public class ConfigureCosmosOptions : IConfigureOptions<CosmosOptions>
{
    public void Configure(CosmosOptions options)
    {
        options.UseCosmosEmulator();
        options.DatabaseName = "CQRS";
    }
}