namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventStore(
        this IServiceCollection services,
        Action<EventStoreOptionsBuilder> configure)
    {
        services.TryAddSingleton<CosmosEventSerializer>();
        services.TryAddSingleton<IEventStoreClient, EventStoreClient>();

        var configureOptions = new EventStoreOptionsBuilder(services);
        configure?.Invoke(configureOptions);

        services.TryAddSingleton<IDateTimeProvider, UtcDateTimeProvider>();

        services.TryAddSingleton<IStreamWriteValidator, StreamWriteValidator>();
        services.TryAddSingleton<IStreamReadValidator, StreamReadValidator>();
        services.TryAddSingleton<IEventBatchProducer, EventBatchProducer>();

        services.TryAddSingleton<IStreamInfoReader, StreamInfoReader>();
        services.TryAddSingleton<IStreamReader, StreamReader>();
        services.TryAddSingleton<IStreamWriter, StreamWriter>();

        services.TryAddSingleton<ISubscriptionProcessorTelemetry, SubscriptionProcessorTelemetry>();

        return services;
    }
}