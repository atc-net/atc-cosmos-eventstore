namespace GettingStarted.Options;

public class ConfigureEventStoreOptions : IConfigureOptions<EventStoreClientOptions>
{
    public void Configure(EventStoreClientOptions options)
    {
        options.UseCosmosEmulator();
        options.EventStoreDatabaseId = "CQRS";
    }
}