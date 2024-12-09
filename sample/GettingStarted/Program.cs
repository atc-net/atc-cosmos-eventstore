using Atc.Cosmos;
using Atc.Cosmos.EventStore;
using GettingStarted;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Atc.Cosmos.Internal;
using GettingStarted.Storage;
using Microsoft.Extensions.Options;

void ConfigureServices(IServiceCollection services)
{
    services.ConfigureOptions<ConfigureCosmosOptions>();
    services.ConfigureCosmos(builder =>
    {
        builder.AddContainer<ContainerInitializer, SampleReadModel>(ContainerInitializer.Name);
        builder.UseHostedService();
    });

    services.ConfigureOptions<ConfigureEventStoreOptions>();
    services.AddEventStore(
        builder =>
        {
            builder.UseCosmosDb();
            builder.UseEvents(catalogBuilder => catalogBuilder.FromAssembly<AddedEvent>());
            builder.UseCQRS(
                c =>
                {
                    c.AddInitialization(
                        4000,
                        serviceProvider => serviceProvider
                            .GetRequiredService<ICosmosInitializer>()
                            .InitializeAsync(CancellationToken.None));

                    c.AddCommandsFromAssembly<CreateCommand>();
                    c.AddProjectionJob<SampleProjection>(nameof(SampleProjection));
                });
        });

    services.AddHostedService<ConsoleHostedService>();
}

await Host.CreateDefaultBuilder()
    .ConfigureServices(ConfigureServices)
    .RunConsoleAsync();

public class ConfigureCosmosOptions : IConfigureOptions<CosmosOptions>
{
    public void Configure(CosmosOptions options)
    {
        options.UseCosmosEmulator();
        options.DatabaseName = "CQRS";
    }
}

public class ConfigureEventStoreOptions : IConfigureOptions<EventStoreClientOptions>
{
    public void Configure(EventStoreClientOptions options)
    {
        options.UseCosmosEmulator();
        options.EventStoreDatabaseId = "CQRS";
    }
}