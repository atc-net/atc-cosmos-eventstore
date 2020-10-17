using System.Threading;
using System.Threading.Tasks;
using BigBang.Cosmos.EventStore.Readers;
using Microsoft.Azure.Cosmos;

namespace BigBang.Cosmos.EventStore.Writers
{
    public class SingleEventStreamWriter
    {
        private readonly CosmosSerializer serializer;
        private readonly EventProducer eventProducer;
        private readonly Container container;
        private readonly MetadataReader metadataReader;

        public SingleEventStreamWriter(
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
            object @event,
            string? correlationId = null,
            CancellationToken cancellationToken = default)
        {
            if (version == ExpectedVersion.Any)
            {
                version = await metadataReader.GetStreamVersionAsync(streamName, streamId, cancellationToken)
                    .ConfigureAwait(false);
            }

            return await WriteSingleEventAsync(
                 eventProducer.Produce(@event, streamId, streamName, version, correlationId), cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task<EventStreamResponse> WriteSingleEventAsync(Event @event, CancellationToken cancellationToken)
        {
            var partitionKey = new PartitionKey(@event.PartitionKey);

            using var stream = serializer.ToStream(@event);
            using var response = await container
                .CreateItemStreamAsync(
                    stream,
                    partitionKey,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            var responseEnvelope = response.IsSuccessStatusCode
                ? serializer.FromStream<Event>(response.Content)
                : null;

            return responseEnvelope is null
                 ? new EventStreamResponse(
                     response.ErrorMessage,
                     response.StatusCode,
                     @event.Properties.StreamId,
                     @event.Properties.Etag)
                 : new EventStreamResponse(
                     string.Empty,
                     response.StatusCode,
                     responseEnvelope.Properties.StreamId,
                     responseEnvelope.Properties.Etag);
        }
    }
}