using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BigBang.Cosmos.EventStore.Readers;
using BigBang.Cosmos.EventStore.Writers;
using Microsoft.Azure.Cosmos;

namespace BigBang.Cosmos.EventStore
{
    public class EventStream : IEventStream
    {
        private readonly EventStreamReader streamReader;
        private readonly SingleEventStreamWriter eventWriter;
        private readonly BatchEventStreamWriter batchWriter;
        private readonly EventStreamChangeFeedReader changeFeedReader;
        private readonly EventStreamChangeFeedProcessor changeFeedProcessor;
        private readonly string streamName;

        public EventStream(
            Container streamContainer,
            Container leaseContainer,
            CosmosSerializer serializer,
            StreamType streamType,
            string streamName)
        {
            this.streamName = streamName;
            var eventProducer = new EventProducer(streamType);
            streamReader = new EventStreamReader(streamContainer, serializer, streamType);
            changeFeedReader = new EventStreamChangeFeedReader(streamContainer, leaseContainer);
            changeFeedProcessor = new EventStreamChangeFeedProcessor(streamContainer, leaseContainer);

            var metadataReader = new MetadataReader(streamContainer, serializer);
            eventWriter = new SingleEventStreamWriter(streamContainer, serializer, eventProducer, metadataReader);
            batchWriter = new BatchEventStreamWriter(streamContainer, serializer, eventProducer, metadataReader);
        }

        public EventStreamSubscription CreateOrResumeSubscription(
            string name,
            SubscriptionDelegate callback,
            string? instanceName = null,
            TimeSpan? pollingInterval = null)
            => changeFeedProcessor.GetSubscription(
                name,
                instanceName ?? "default",
                pollingInterval ?? TimeSpan.FromSeconds(1),
                callback);

        public IAsyncEnumerable<IReadOnlyCollection<Event>> CountinuousReadStreamAsync(
            string name,
            string? instanceName = null,
            TimeSpan? pollingInterval = default,
            CancellationToken cancellationToken = default)
            => changeFeedReader.FetchAsync(
                name,
                instanceName ?? "default",
                pollingInterval ?? TimeSpan.FromSeconds(1),
                cancellationToken);

        public IAsyncEnumerable<IReadOnlyCollection<Event>> ReadStreamAsync(
            string streamId,
            string? fromEtag = null,
            CancellationToken cancellationToken = default)
            => streamReader.FetchAsync(
                streamName.ThrowIfStreamNameIsInvalid(),
                streamId.ThrowIfStreamIdIsInvalid(),
                fromEtag.GetVersionFromEtag(streamId),
                cancellationToken);

        public Task<EventStreamResponse> WriteEventsAsync(
            string streamId,
            IReadOnlyCollection<object> events,
            string? etag = null,
            string? correlationId = null,
            CancellationToken cancellationToken = default)
        {
            streamId.ThrowIfStreamIdIsInvalid();
            var version = etag.GetVersionFromEtag(streamId);

            return events.Count == 1
                 ? eventWriter.WriteAsync(streamId, streamName, version, events.First(), correlationId, cancellationToken)
                 : batchWriter.WriteAsync(streamId, streamName, version, events, correlationId, cancellationToken);
        }
    }
}