using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BigBang.Cosmos.EventStore.Readers;
using Microsoft.Azure.Cosmos;

namespace BigBang.Cosmos.EventStore.Writers
{
    public class BatchEventStreamWriter
    {
        private readonly CosmosSerializer serializer;
        private readonly EventProducer eventProducer;
        private readonly Container container;
        private readonly MetadataReader metadataReader;

        public BatchEventStreamWriter(
            Container container,
            CosmosSerializer serializer,
            EventProducer eventProducer,
            MetadataReader metadataReader)
        {
            this.container = container;
            this.serializer = serializer;
            this.metadataReader = metadataReader;
            this.eventProducer = eventProducer;
        }

        public async Task<EventStreamResponse> WriteAsync(
            string streamId,
            string streamName,
            long version,
            IReadOnlyCollection<object> events,
            string? correlationId = null,
            CancellationToken cancellationToken = default)
        {
            if (version == ExpectedVersion.Any)
            {
                version = await metadataReader.GetStreamVersionAsync(streamName, streamId, cancellationToken)
                    .ConfigureAwait(false);
            }

            return await WriteBatchEventsAsync(
                streamId,
                eventProducer.Produce(events, streamId, streamName, version, correlationId),
                cancellationToken)
            .ConfigureAwait(false);
        }

        private async Task<EventStreamResponse> WriteBatchEventsAsync(string streamId, IEnumerable<Event> events, CancellationToken cancellationToken = default)
        {
            var partitionKey = new PartitionKey(streamId);
            var batch = container.CreateTransactionalBatch(partitionKey);
            foreach (var @event in events)
            {
                batch.CreateItemStream(serializer.ToStream(@event));
            }

            var response = await batch.ExecuteAsync(cancellationToken).ConfigureAwait(false);
            var responseEnvelope = response.IsSuccessStatusCode
                ? serializer.FromStream<Event>(response[^1].ResourceStream)
                : null;

            return responseEnvelope is null
                 ? new EventStreamResponse(
                     response.ErrorMessage,
                     response.StatusCode,
                     events.First().Properties.StreamId,
                     events.First().Properties.Etag)
                 : new EventStreamResponse(
                     string.Empty,
                     response[^1].StatusCode,
                     responseEnvelope.Properties.StreamId,
                     responseEnvelope.Properties.Etag);
        }
    }
}