using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BigBang.Cosmos.EventStore.Serialization;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;

namespace BigBang.Cosmos.EventStore
{
    public class EventStoreClient : IEventStoreClient
    {
        public const string VersionedStreamContainerName = "versioned-streams";
        public const string TimeSeriesStreamContainerName = "timeseries-streams";
        public const string ProcessorLeasesContainerName = "processor-leases";

        private readonly TextJsonCosmosSerializer serializer;
        private readonly string storeName;
        private readonly int throughput;

        public EventStoreClient(EventStoreClientOptions options)
            : this (options.CosmosBuilder, options.StoreName, options.Throughput, options.TypeToNameMapper)
        { }

        public EventStoreClient(
            CosmosClientBuilder? builder,
            string storeName,
            int throughput,
            Dictionary<Type, string> typeToName)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            this.storeName = storeName;
            this.throughput = throughput;

            serializer = new TextJsonCosmosSerializer(new EventTypeNameMapper(typeToName));
            Client = builder
                .WithCustomSerializer(serializer)
                .Build();
        }

        public EventStoreClient(
            string accountEndpoint,
            string authKey,
            string storeName,
            int throughput,
            Dictionary<Type, string> typeToName)
            : this(new CosmosClientBuilder(accountEndpoint, authKey), storeName, throughput, typeToName)
        { }

        protected CosmosClient Client { get; set; }
        protected CosmosSerializer Serializer => serializer;

        public virtual async Task InitializeStoreAsync(CancellationToken cancellationToken = default)
        {
            await GetOrCreateStreamContainerAsync(VersionedStreamContainerName, cancellationToken);
            await GetOrCreateStreamContainerAsync(TimeSeriesStreamContainerName, cancellationToken);
            await GetOrCreateStreamProcessorContainerAsync(ProcessorLeasesContainerName, cancellationToken);
        }

        public virtual IEventStream GetTimeseriesStream(string streamName)
            => new EventStream(
                Client.GetContainer(storeName, TimeSeriesStreamContainerName),
                Client.GetContainer(storeName, ProcessorLeasesContainerName),
                serializer,
                StreamType.Timeseries,
                streamName.ThrowIfStreamNameIsInvalid());

        public virtual IEventStream GetVersionedStream(string streamName)
            => new EventStream(
                Client.GetContainer(storeName, VersionedStreamContainerName),
                Client.GetContainer(storeName, ProcessorLeasesContainerName),
                serializer,
                StreamType.Versioned,
                streamName.ThrowIfStreamNameIsInvalid());

        private async Task<Container> GetOrCreateStreamContainerAsync(string name, CancellationToken cancellationToken)
        {
            var database = await GetOrCreateDatabaseAsync(cancellationToken);

            return await database.DefineContainer(name, "/pk")
                    ////.WithUniqueKey()
                    ////    .Path("/properties/streamId")
                    ////    .Path("/properties/version")
                    ////    .Attach()
                    .WithIndexingPolicy()
                        .WithAutomaticIndexing(true)
                        .WithIndexingMode(IndexingMode.Consistent)
                        .WithExcludedPaths()
                            .Path("/data/*")
                            .Attach()
                        .WithIncludedPaths()
                            .Path("/")
                            .Attach()
                        .Attach()
                    .CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        }

        private async Task<Container> GetOrCreateStreamProcessorContainerAsync(string name, CancellationToken cancellationToken)
        {
            var database = await GetOrCreateDatabaseAsync(cancellationToken);

            return await database.CreateContainerIfNotExistsAsync(
                name,
                "/id",
                cancellationToken: cancellationToken);
        }

        private async Task<Database> GetOrCreateDatabaseAsync(CancellationToken cancellationToken)
        {
            var response = await Client.CreateDatabaseIfNotExistsAsync(
                    storeName,
                    throughput,
                    cancellationToken: cancellationToken);

            return response.Database;
        }
    }
}