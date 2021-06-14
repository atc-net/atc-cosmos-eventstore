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

        public Task DeleteSubscribeAsync(ConsumerGroup consumerGroup, CancellationToken cancellationToken = default)
            => subscriptionRemover.DeleteAsync(
                Arguments.EnsureNotNull(consumerGroup, nameof(consumerGroup)),
                cancellationToken);

        public Task<IStreamMetadata> GetStreamInfoAsync(
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
            => subscriptionFactory
                .Create(
                    Arguments.EnsureNotNull(consumerGroup, nameof(consumerGroup)),
                    startOptions,
                    Arguments.EnsureNotNull(eventsHandler, nameof(eventsHandler)),
                    Arguments.EnsureNotNull(errorHandler, nameof(errorHandler)));

        public Task<StreamResponse> WriteToStreamAsync(
            StreamId streamId,
            IReadOnlyCollection<object> events,
            StreamVersion version,
            StreamWriteOptions? options = null,
            CancellationToken cancellationToken = default)
            => streamWriter.WriteAsync(
                streamId,
                Arguments.EnsureNoNullValues(events, nameof(events)),
                version,
                options,
                cancellationToken);
    }
}