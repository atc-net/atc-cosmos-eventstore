using System;
using System.Collections.Generic;
using System.Linq;
using Atc.Cosmos.EventStore.Streams;

namespace Atc.Cosmos.EventStore.Events
{
    public class EventBatchProducer : IEventBatchProducer
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly IEventIdProvider eventIdProvider;
        private readonly IEventNameProvider nameProvider;

        public EventBatchProducer(
            IDateTimeProvider dateTimeProvider,
            IEventIdProvider eventIdProvider,
            IEventNameProvider nameProvider)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.eventIdProvider = eventIdProvider;
            this.nameProvider = nameProvider;
        }

        public StreamBatch FromEvents(
            IReadOnlyCollection<object> events,
            IStreamMetadata metadata,
            StreamWriteOptions? options)
        {
            var timestamp = dateTimeProvider.GetDateTime();
            var version = metadata.Version.Value;

            var documents = events
                .Select(evt => Convert(
                    evt,
                    metadata,
                    ++version, // increment version for event
                    options?.CorrelationId,
                    options?.CausationId,
                    timestamp))
                .ToArray();

            return new StreamBatch(
                new StreamMetadata(
                    StreamMetadata.StreamMetadataId,
                    metadata.StreamId.Value,
                    metadata.StreamId,
                    version,
                    StreamState.Active,
                    timestamp)
                {
                    ETag = metadata.ETag,
                },
                documents);
        }

        private EventDocument Convert(
            object evt,
            IStreamMetadata metadata,
            long version,
            string? correlationId,
            string? causationId,
            DateTimeOffset timestamp)
        {
            var eventId = eventIdProvider.CreateUniqueId(metadata);
            var streamId = metadata.StreamId.Value;
            var name = nameProvider.GetName(evt);

            return new EventDocument<object>
            {
                Id = eventId,
                PartitionKey = streamId,
                Data = evt,
                Properties = new EventProperties
                {
                    CausationId = causationId,
                    CorrelationId = correlationId,
                    EventId = eventId,
                    StreamId = streamId,
                    Version = version,
                    Timestamp = timestamp,
                    Name = name,
                },
            };
        }
    }
}