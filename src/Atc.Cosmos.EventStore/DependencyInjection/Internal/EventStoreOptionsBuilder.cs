using System;
using Atc.Cosmos.EventStore.Cosmos;
using Atc.Cosmos.EventStore.Events;
using Atc.Cosmos.EventStore.InMemory;
using Atc.Cosmos.EventStore.Streams;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Atc.Cosmos.EventStore.DependencyInjection.Internal
{
    public class EventStoreOptionsBuilder
    {
        public EventStoreOptionsBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }

        /// <summary>
        /// Configure event store with default options.
        /// </summary>
        /// <returns>Option builder.</returns>
        public EventStoreOptionsBuilder UseCosmosDb()
            => UseCosmosDb(o => { });

        public EventStoreOptionsBuilder UseCosmosDb(
            Action<EventStoreClientOptions> configure)
        {
            Services.Configure(configure);

            Services.TryAddSingleton<ICosmosClientFactory, CosmosClientFactory>();
            Services.TryAddSingleton<IEventStoreContainerProvider, CosmosContainerProvider>();
            Services.TryAddSingleton<IEventStoreInitializer, CosmosEventStoreInitializer>();

            Services.TryAddSingleton<IStreamMetadataReader, CosmosMetadataReader>();
            Services.TryAddSingleton<IStreamIterator, CosmosStreamIterator>();
            Services.TryAddSingleton<IStreamBatchWriter, CosmosBatchWriter>();
            Services.TryAddSingleton<IStreamSubscriptionProvider, CosmosSubscriptionProvider>();
            Services.TryAddSingleton<IStreamSubscriptionRemover, CosmosSubscriptionRemover>();

            return this;
        }

        public EventStoreOptionsBuilder UseInMemoryDb()
        {
            Services.TryAddSingleton<IEventStoreInitializer, InMemoryStoreInitializer>();

            Services.TryAddSingleton<InMemoryStore>();
            Services.TryAddSingleton<IStreamMetadataReader>(s => s.GetRequiredService<InMemoryStore>());
            Services.TryAddSingleton<IStreamIterator>(s => s.GetRequiredService<InMemoryStore>());
            Services.TryAddSingleton<IStreamBatchWriter>(s => s.GetRequiredService<InMemoryStore>());
            Services.TryAddSingleton<IStreamSubscriptionProvider>(s => s.GetRequiredService<InMemoryStore>());
            Services.TryAddSingleton<IStreamSubscriptionRemover>(s => s.GetRequiredService<InMemoryStore>());

            return this;
        }

        public EventStoreOptionsBuilder UseCustomDateTimeProvider<T>()
            where T : class, IDateTimeProvider
        {
            Services.TryAddSingleton<IDateTimeProvider, T>();

            return this;
        }

        public EventStoreOptionsBuilder UseCustomEventIdProvider<T>()
            where T : class, IEventIdProvider
        {
            Services.TryAddSingleton<IEventIdProvider, T>();

            return this;
        }

        public EventStoreOptionsBuilder UseEvents(Action<IEventCatalogBuilder> configure)
        {
            var builder = new EventCatalogBuilder();

            configure?.Invoke(builder);

            var catalog = builder.Build();

            Services.TryAddSingleton<IEventNameProvider>(catalog);
            Services.TryAddSingleton<IEventTypeProvider>(catalog);

            return this;
        }
    }
}