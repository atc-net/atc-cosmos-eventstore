using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Atc.Cosmos.EventStore.Streams;

namespace Atc.Cosmos.EventStore
{
    public class EventStoreClient : IEventStoreClient
    {
        private readonly IStreamWriter streamWriter;
        private readonly IStreamReader streamReader;
        private readonly IStreamInfoReader infoReader;
        private readonly IStreamSubscriptionFactory subscriptionFactory;
        private readonly IStreamSubscriptionRemover subscriptionRemover;

        public EventStoreClient(
            IStreamWriter streamWriter,
            IStreamReader streamReader,
            IStreamInfoReader infoReader,
            IStreamSubscriptionFactory subscriptionFactory,
            IStreamSubscriptionRemover subscriptionRemover)
        {
            this.streamWriter = streamWriter;
            this.streamReader = streamReader;
            this.infoReader = infoReader;
            this.subscriptionFactory = subscriptionFactory;
            this.subscriptionRemover = subscriptionRemover;
        }

        public ValueTask DeleteSubscribeAsync(ConsumerGroup consumerGroup, CancellationToken cancellationToken = default)
        {
            Arguments.EnsureNotNull(consumerGroup, nameof(consumerGroup));

            return subscriptionRemover.DeleteAsync(consumerGroup, cancellationToken);
        }

        public ValueTask<IStreamMetadata> GetStreamInfoAsync(
            StreamId streamId,
            CancellationToken cancellationToken = default)
            => infoReader.ReadAsync(streamId, cancellationToken);

        public IAsyncEnumerable<IEvent> ReadFromStreamAsync(
            StreamId streamId,
            StreamVersion? fromVersion = null,
            CancellationToken cancellationToken = default)
            => streamReader.ReadAsync(
                streamId,
                Arguments.EnsureValueRange(fromVersion ?? StreamVersion.Any, nameof(fromVersion)),
                cancellationToken);

        public IStreamSubscription SubscribeToStreams(
            ConsumerGroup consumerGroup,
            SubscriptionStartOptions startOptions,
            ProcessEventsHandler eventsHandler,
            ProcessExceptionHandler errorHandler)
        {
            Arguments.EnsureNotNull(consumerGroup, nameof(consumerGroup));
            Arguments.EnsureNotNull(eventsHandler, nameof(eventsHandler));
            Arguments.EnsureNotNull(errorHandler, nameof(errorHandler));

            return subscriptionFactory
                .Create(
                    consumerGroup,
                    startOptions,
                    eventsHandler,
                    errorHandler);
        }

        public ValueTask<StreamResponse> WriteToStreamAsync(
            StreamId streamId,
            IReadOnlyCollection<object> events,
            StreamVersion version,
            StreamWriteOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            Arguments.EnsureNoNullValues(events, nameof(events));

            return streamWriter.WriteAsync(
                streamId,
                events,
                version,
                options,
                cancellationToken);
        }
    }
}