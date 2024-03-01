using Atc.Cosmos.EventStore;
using Microsoft.Extensions.Options;

namespace GettingStarted;

public class ConfigureEventStoreOptions : IConfigureOptions<EventStoreClientOptions>
{
    public void Configure(EventStoreClientOptions options)
    {
        options.UseCosmosEmulator();
        options.EventStoreDatabaseId = "CQRS";
    }
}