using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Atc.Cosmos.EventStore.Streams;
using Microsoft.Azure.Cosmos;

namespace Atc.Cosmos.EventStore.Cosmos
{
    public class CosmosSubscriptionRemover : IStreamSubscriptionRemover
    {
        private readonly IEventStoreContainerProvider containerProvider;

        public CosmosSubscriptionRemover(IEventStoreContainerProvider containerProvider)
        {
            this.containerProvider = containerProvider;
        }

        public async ValueTask DeleteAsync(ConsumerGroup consumerGroup, CancellationToken cancellationToken)
        {
            var resultSet = containerProvider
                .GetSubscriptionContainer()
                .GetItemQueryIterator<SubscriptionLease>(
                    new QueryDefinition("SELECT r.id FROM r WHERE STARTSWITH(r.id, @name, false)")
                        .WithParameter("@name", GetProcessorName(consumerGroup)));

            while (resultSet.HasMoreResults)
            {
                var registrations = await resultSet
                    .ReadNextAsync(cancellationToken)
                    .ConfigureAwait(false);
                foreach (var registration in registrations)
                {
                    await containerProvider
                        .GetSubscriptionContainer()
                        .DeleteItemAsync<object>(
                            registration.Id,
                            new PartitionKey(registration.Id),
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
            }
        }

        private static string GetProcessorName(ConsumerGroup consumerGroup)
            => consumerGroup.Name + ":";

        internal class SubscriptionLease
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;
        }
    }
}