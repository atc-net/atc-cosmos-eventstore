using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Atc.Cosmos.EventStore.Cosmos
{
    internal class CosmosEventStoreInitializer : IEventStoreInitializer
    {
        private readonly EventStoreClientOptions options;
        private readonly ICosmosClientFactory clientFactory;

        public CosmosEventStoreInitializer(
            IOptions<EventStoreClientOptions> options,
            ICosmosClientFactory clientFactory)
        {
            this.options = options.Value;
            this.clientFactory = clientFactory;
        }

        public void CreateEventStore(ThroughputProperties throughputProperties)
        {
            CreateEventStoreAsync(throughputProperties, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }

        public async Task CreateEventStoreAsync(
            ThroughputProperties throughputProperties,
            CancellationToken cancellationToken)
        {
            await clientFactory
                .GetClient()
                .CreateDatabaseIfNotExistsAsync(
                    options.EventStoreDatabaseId,
                    throughputProperties,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await CreateEventStoreContainerAsync(cancellationToken)
                 .ConfigureAwait(continueOnCapturedContext: false);
            await CreateSubscriptionContainerAsync(cancellationToken)
                 .ConfigureAwait(continueOnCapturedContext: false);
            await CreateIndexContainerAsync(cancellationToken)
                 .ConfigureAwait(continueOnCapturedContext: false);
        }

        private Task CreateEventStoreContainerAsync(CancellationToken cancellationToken)
        {
            var containerOptions = new ContainerProperties
            {
                IndexingPolicy = new IndexingPolicy
                {
                    Automatic = true,
                    IndexingMode = IndexingMode.Consistent,
                },
                Id = options.EventStoreContainerId,
                PartitionKeyPath = "/pk",
            };

            containerOptions.IndexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/*" });

            // Exclude event data from indexing.
            containerOptions.IndexingPolicy.ExcludedPaths.Add(new ExcludedPath { Path = "/data/*" });

            containerOptions.IndexingPolicy.CompositeIndexes.Add(new(
                new[]
                {
                    new CompositePath { Path = "/pk", Order = CompositePathSortOrder.Ascending },
                    new CompositePath { Path = "/properties/version", Order = CompositePathSortOrder.Ascending },
                }.ToList()));

            return clientFactory
                .GetClient()
                .GetDatabase(options.EventStoreDatabaseId)
                .CreateContainerIfNotExistsAsync(
                    containerOptions,
                    cancellationToken: cancellationToken);
        }

        private Task CreateSubscriptionContainerAsync(CancellationToken cancellationToken)
        {
            var containerOptions = new ContainerProperties
            {
                IndexingPolicy = new IndexingPolicy
                {
                    Automatic = true,
                    IndexingMode = IndexingMode.Consistent,
                },
                Id = options.SubscriptionContainerId,
                PartitionKeyPath = "/id",
            };

            return clientFactory
                .GetClient()
                .GetDatabase(options.EventStoreDatabaseId)
                .CreateContainerIfNotExistsAsync(
                    containerOptions,
                    cancellationToken: cancellationToken);
        }

        private Task CreateIndexContainerAsync(CancellationToken cancellationToken)
        {
            var containerOptions = new ContainerProperties
            {
                IndexingPolicy = new IndexingPolicy
                {
                    Automatic = true,
                    IndexingMode = IndexingMode.Consistent,
                },
                Id = options.IndexContainerId,
                PartitionKeyPath = "/pk",
            };

            containerOptions.IndexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/*" });

            // Exclude snapshot data from indexing.
            containerOptions.IndexingPolicy.ExcludedPaths.Add(new ExcludedPath { Path = "/data/*" });

            return clientFactory
                .GetClient()
                .GetDatabase(options.EventStoreDatabaseId)
                .CreateContainerIfNotExistsAsync(
                    containerOptions,
                    cancellationToken: cancellationToken);
        }
    }
}