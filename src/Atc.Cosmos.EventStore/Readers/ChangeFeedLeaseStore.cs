using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace BigBang.Cosmos.EventStore.Readers
{
    public class ChangeFeedLeaseStore
    {
        internal class LeaseRegistration
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;
        }

        private readonly Container leaseContainer;

        public ChangeFeedLeaseStore(Container leaseContainer)
        {
            this.leaseContainer = leaseContainer ?? throw new ArgumentNullException(nameof(leaseContainer));
        }

        public async Task<long> RemoveLeasesAsync(string name, CancellationToken cancellationToken = default)
        {
            var resultSet = leaseContainer.GetItemQueryIterator<LeaseRegistration>(
                new QueryDefinition("SELECT r.id FROM r WHERE STARTSWITH(r.id, @name, false)")
                    .WithParameter("@name", name));

            while (resultSet.HasMoreResults)
            {
                var registrations = await resultSet.ReadNextAsync(cancellationToken);
                foreach (var registration in registrations)
                {
                    await leaseContainer.DeleteItemAsync<object>(
                        registration.Id,
                        new PartitionKey(registration.Id),
                        cancellationToken: cancellationToken);
                }
            }

            return ExpectedVersion.Empty;
        }
    }
}