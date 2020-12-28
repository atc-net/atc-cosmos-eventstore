using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace BigBang.Cosmos.EventStore.Readers
{
    /// <summary>
    /// Responsible for reading last know version of a given stream instance.
    /// </summary>
    public class MetadataReader
    {
        private readonly CosmosSerializer serializer;
        private readonly Container container;

        public MetadataReader(Container container, CosmosSerializer serializer)
        {
            this.container = container;
            this.serializer = serializer;
        }

        public async Task<long> GetStreamVersionAsync(string streamName, string streamId, CancellationToken cancellationToken = default)
        {
            var partitionKey = EventProducer.GetPartitionKey(streamName, streamId);
            var resultSet = container.GetItemQueryStreamIterator(
                GetLastVersionQuery(partitionKey),
                requestOptions: GetOptions(partitionKey));

            if (!resultSet.HasMoreResults)
            {
                return ExpectedVersion.Empty;
            }

            using var responseMessage = await resultSet
                .ReadNextAsync(cancellationToken)
                .ConfigureAwait(false);
            if (!responseMessage.IsSuccessStatusCode)
            {
                return ExpectedVersion.Empty;
            }

            var queryResponse = serializer.FromStream<StreamMetadataResponse>(responseMessage.Content);
            var data = queryResponse
                ?.Documents
                .FirstOrDefault();

            return data is null
                ? ExpectedVersion.Empty
                : data.Version;
        }

        private static QueryRequestOptions GetOptions(string partitionKey)
            => new QueryRequestOptions
            {
                MaxConcurrency = 1,
                PartitionKey = new PartitionKey(partitionKey),
            };

        private static QueryDefinition GetLastVersionQuery(string partitionKey)
            => new QueryDefinition(
                "SELECT TOP 1 e.properties.version FROM events e WHERE e.pk = @id ORDER BY e.properties.version DESC")
                .WithParameter("@id", partitionKey);

        internal class StreamMetadataResponse
        {
            [JsonPropertyName("Documents")]
            public List<StreamVersion> Documents { get; set; } = new List<StreamVersion>();
        }

        internal class StreamVersion
        {
            [JsonPropertyName("version")]
            public long Version { get; set; }
        }
    }
}