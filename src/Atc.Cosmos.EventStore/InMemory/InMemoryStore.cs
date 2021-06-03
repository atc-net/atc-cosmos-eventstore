using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Atc.Cosmos.EventStore.Events;
using Atc.Cosmos.EventStore.Streams;

namespace Atc.Cosmos.EventStore.InMemory
{
    public class InMemoryStore :
        IStreamMetadataReader,
        IStreamIterator,
        IStreamBatchWriter,
        IStreamSubscriptionProvider,
        IStreamSubscriptionRemover
    {
        public IStreamSubscription Create(
            ConsumerGroup consumerGroup,
            SubscriptionStartOptions startOptions,
            Func<IReadOnlyCollection<EventDocument>, CancellationToken, Task> changes)
            => throw new NotSupportedException();

        public ValueTask DeleteAsync(
            ConsumerGroup consumerGroup,
            CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public ValueTask<IStreamMetadata> GetAsync(
            StreamId streamId,
            CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public IAsyncEnumerable<IEvent> ReadAsync(
            StreamId streamId,
            StreamVersion fromVersion,
            CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public ValueTask<IStreamMetadata> WriteAsync(
            StreamBatch batch,
            CancellationToken cancellationToken)
            => throw new NotSupportedException();
    }
}