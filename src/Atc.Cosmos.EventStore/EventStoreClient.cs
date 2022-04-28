using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Atc.Cosmos.EventStore.Streams;

namespace Atc.Cosmos.EventStore
{
    internal class EventStoreClient : IEventStoreClient
    {
        private readonly IStreamWriter streamWriter;
        private readonly IStreamReader streamReader;
        private readonly IStreamInfoReader infoReader;
        private readonly IStreamIndexReader indexReader;
        private readonly IStreamCheckpointWriter checkpointWriter;
        private readonly IStreamCheckpointReader checkpointReader;
        private readonly IStreamSubscriptionFactory subscriptionFactory;
        private readonly IStreamSubscriptionRemover subscriptionRemover;

        public EventStoreClient(
            IStreamWriter streamWriter,
            IStreamReader streamReader,
            IStreamInfoReader infoReader,
            IStreamIndexReader indexReader,
            IStreamCheckpointWriter checkpointWriter,
            IStreamCheckpointReader checkpointReader,
            IStreamSubscriptionFactory subscriptionFactory,
            IStreamSubscriptionRemover subscriptionRemover)
        {
            this.streamWriter = streamWriter;
            this.streamReader = streamReader;
            this.infoReader = infoReader;
            this.indexReader = indexReader;
            this.checkpointWriter = checkpointWriter;
            this.checkpointReader = checkpointReader;
            this.subscriptionFactory = subscriptionFactory;
            this.subscriptionRemover = subscriptionRemover;
        }

        public Task DeleteSubscriptionAsync(
            ConsumerGroup consumerGroup,
            CancellationToken cancellationToken = default)
            => subscriptionRemover
                .DeleteAsync(
                    Arguments.EnsureNotNull(consumerGroup, nameof(consumerGroup)),
                    cancellationToken);

        public Task<IStreamMetadata> GetStreamInfoAsync(
            StreamId streamId,
            CancellationToken cancellationToken = default)
            => infoReader
                .ReadAsync(
                    streamId,
                    cancellationToken);

        public IAsyncEnumerable<IStreamIndex> QueryStreamsAsync(
            string? filter = default,
            DateTimeOffset? createdAfter = default,
            CancellationToken cancellationToken = default)
            => indexReader
                .ReadAsync(
                    filter,
                    createdAfter,
                    cancellationToken);

        public IAsyncEnumerable<IEvent> ReadFromStreamAsync(
            StreamId streamId,
            StreamVersion? fromVersion = default,
            StreamReadOptions? options = default,
            CancellationToken cancellationToken = default)
            => streamReader
                .ReadAsync(
                    streamId,
                    Arguments.EnsureValueRange(fromVersion ?? StreamVersion.Any, nameof(fromVersion)),
                    options,
                    cancellationToken);

        public IStreamSubscription SubscribeToStreams(
            ConsumerGroup consumerGroup,
            SubscriptionStartOptions startOptions,
            ProcessEventsHandler eventsHandler)
            => subscriptionFactory
                .Create(
                    Arguments.EnsureNotNull(consumerGroup, nameof(consumerGroup)),
                    startOptions,
                    Arguments.EnsureNotNull(eventsHandler, nameof(eventsHandler)));

        public Task<StreamResponse> WriteToStreamAsync(
            StreamId streamId,
            IReadOnlyCollection<object> events,
            StreamVersion? version = default,
            StreamWriteOptions? options = default,
            CancellationToken cancellationToken = default)
            => streamWriter
                .WriteAsync(
                    streamId,
                    Arguments.EnsureNoNullValues(events, nameof(events)),
                    version ?? StreamVersion.Any,
                    options,
                    cancellationToken);

        public Task SetStreamCheckpointAsync(
            string name,
            StreamId streamId,
            StreamVersion version,
            object? state = null,
            CancellationToken cancellationToken = default)
            => checkpointWriter
                .WriteAsync(
                    Arguments.EnsureNotNull(name, nameof(name)),
                    streamId,
                    version,
                    state,
                    cancellationToken);

        public Task<Checkpoint<T>?> GetStreamCheckpointAsync<T>(
            string name,
            StreamId streamId,
            CancellationToken cancellationToken = default)
            => checkpointReader
                .ReadAsync<T>(
                    Arguments.EnsureNotNull(name, nameof(name)),
                    streamId,
                    cancellationToken);

        public async Task<Checkpoint?> GetStreamCheckpointAsync(
            string name,
            StreamId streamId,
            CancellationToken cancellationToken = default)
            => await checkpointReader
                .ReadAsync<object>(
                    Arguments.EnsureNotNull(name, nameof(name)),
                    streamId,
                    cancellationToken)
                .ConfigureAwait(false);
    }
}