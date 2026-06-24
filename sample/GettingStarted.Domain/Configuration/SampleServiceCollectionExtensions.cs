namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Shared composition for the sample: wires up Atc.Cosmos read models and the
/// Atc.Cosmos.EventStore CQRS pipeline against the Cosmos DB Emulator. Both the
/// console worker and the Web API use this so they share a single setup.
/// </summary>
public static class SampleServiceCollectionExtensions
{
    public static IServiceCollection AddSampleEventStore(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Read-model store. AccountEndpoint/AccountKey come from CosmosOptions__* env vars
        // injected by the AppHost (or the emulator defaults when running standalone).
        services.Configure<CosmosOptions>(configuration.GetSection(nameof(CosmosOptions)));
        services.ConfigureOptions<ConfigureCosmosOptions>();
        services.ConfigureOptions<ConfigureCosmosClientOptions>();
        services.ConfigureCosmos(cosmos =>
        {
            cosmos.AddContainer<ContainerInitializer, SampleReadModel>(ContainerInitializer.Name);
            cosmos.UseHostedService();
        });

        // Event store + CQRS.
        services.ConfigureOptions<ConfigureEventStoreOptions>();
        services.AddEventStore(eventStore =>
        {
            eventStore.UseCosmosDb();
            eventStore.UseEvents(c => c.FromAssembly<AddedEvent>());
            eventStore.UseCQRS(c =>
            {
                c.AddInitialization(
                    throughput: 4000,
                    sp => sp.GetRequiredService<ICosmosInitializer>()
                        .InitializeAsync(CancellationToken.None));

                c.AddCommandsFromAssembly<CreateCommand>();
                c.AddProjectionJob<SampleProjection>(nameof(SampleProjection));
            });
        });

        return services;
    }
}