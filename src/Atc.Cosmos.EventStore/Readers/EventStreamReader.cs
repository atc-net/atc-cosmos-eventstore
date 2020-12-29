using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Threading;
using Microsoft.Azure.Cosmos;

namespace BigBang.Cosmos.EventStore.Readers
{
    public class EventStreamReader
    {
        private readonly CosmosSerializer serializer;
        private readonly StreamType streamType;
        private readonly Container container;

        public EventStreamReader(
            Container container,
            CosmosSerializer serializer,
            StreamType streamType)
        {
            this.container = container;
            this.serializer = serializer;
            this.streamType = streamType;
        }

        public async IAsyncEnumerable<IReadOnlyCollection<Event>> FetchAsync(
            string streamName,
            string streamId,
            long fromVersion,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var partitionKey = EventProducer.GetPartitionKey(streamName, streamId);
            var resultSet = container.GetItemQueryStreamIterator(
                streamType == StreamType.Versioned
                    ? GetQuery(partitionKey, fromVersion)
                    : GetTimeSeriesQuery(partitionKey),
                requestOptions: GetOptions(partitionKey));

            while (resultSet.HasMoreResults && !cancellationToken.IsCancellationRequested)
            {
                using var responseMessage = await resultSet
                    .ReadNextAsync(cancellationToken)
                    .ConfigureAwait(false);

                responseMessage.EnsureSuccessStatusCode();

                var queryResponse = serializer.FromStream<EventStreamQueryResponse>(responseMessage.Content);
                if (queryResponse?.Events is null)
                {
                    continue;
                }

                yield return queryResponse.Events;
            }
        }

        private static QueryRequestOptions GetOptions(string partitionKey)
            => new QueryRequestOptions
            {
                MaxConcurrency = 1,
                PartitionKey = new PartitionKey(partitionKey),
            };

        private static QueryDefinition GetQuery(string partitionKey, long fromVersion)
            => fromVersion == ExpectedVersion.Any
             ? GetAllVersionsQuery(partitionKey)
             : GetFromVersionQuery(partitionKey, fromVersion);

        private static QueryDefinition GetAllVersionsQuery(string partitionKey)
            => new QueryDefinition(
                "SELECT * FROM events e WHERE e.pk = @partitionKey ORDER BY e.properties.version")
                .WithParameter("@partitionKey", partitionKey);

        private static QueryDefinition GetFromVersionQuery(string partitionKey, long fromVersion)
            => new QueryDefinition(
                "SELECT * FROM events e WHERE e.pk = @partitionKey AND e.properties.version >= @fromVersion ORDER BY e.properties.version")
                .WithParameter("@partitionKey", partitionKey)
                .WithParameter("@fromVersion", fromVersion);

        private static QueryDefinition GetTimeSeriesQuery(string partitionKey)
            => new QueryDefinition(
                "SELECT * FROM events e WHERE e.pk = @partitionKey ORDER BY e._ts")
                .WithParameter("@partitionKey", partitionKey);

        internal class EventStreamQueryResponse
        {
            [JsonPropertyName("Documents")]
            public List<Event> Events { get; set; } = new List<Event>();
        }
    }
}