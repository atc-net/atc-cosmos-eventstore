using System;
using Atc.Cosmos.EventStore;
using Atc.Cosmos.EventStore.Cosmos;
using Atc.Cosmos.EventStore.DependencyInjection.Internal;
using Atc.Cosmos.EventStore.Diagnostics;
using Atc.Cosmos.EventStore.Events;
using Atc.Cosmos.EventStore.Streams;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventStore(
            this IServiceCollection services,
            Action<EventStoreOptionsBuilder> configure)
        {
            services.TryAddSingleton<EventStoreCosmosBuilder>();
            services.TryAddSingleton<CosmosEventSerializer>();
            services.TryAddSingleton<IEventStoreClient, EventStoreClient>();

            var configureOptions = new EventStoreOptionsBuilder(services);
            configure?.Invoke(configureOptions);

            services.TryAddSingleton<IDateTimeProvider, UtcDateTimeProvider>();
            services.TryAddSingleton<IEventIdProvider, GuidEventIdProvider>();

            services.TryAddSingleton<IStreamWriteValidator, StreamWriteValidator>();
            services.TryAddSingleton<IStreamReadValidator, StreamReadValidator>();
            services.TryAddSingleton<IEventBatchProducer, EventBatchProducer>();

            services.TryAddSingleton<IStreamSubscriptionFactory, StreamSubscriptionFactory>();
            services.TryAddSingleton<IStreamInfoReader, StreamInfoReader>();
            services.TryAddSingleton<IStreamReader, StreamReader>();
            services.TryAddSingleton<IStreamWriter, StreamWriter>();

            services.TryAddSingleton<ISubscriptionTelemetry, SubscriptionTelemetry>();

            return services;
        }
    }
}