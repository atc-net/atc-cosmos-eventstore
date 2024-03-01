using GettingStarted;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Atc.Cosmos.Internal;

static void ConfigureServices(
    HostBuilderContext context,
    IServiceCollection services)
{
    services.ConfigureOptions<ConfigureCosmosOptions>();
    services.ConfigureCosmos(builder => builder.UseHostedService());
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
                    // c.AddProjectionJob<TokenProjection>(nameof(SampleProjection));
                });
        });

    services.AddHostedService<ConsoleHostedService>();
}

await Host.CreateDefaultBuilder()
    .ConfigureServices(ConfigureServices)
    .RunConsoleAsync();