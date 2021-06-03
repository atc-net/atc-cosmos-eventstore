using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Atc.Cosmos.EventStore.Events;

namespace Atc.Cosmos.EventStore.Streams
{
    public interface IStreamSubscriptionProvider
    {
        IStreamSubscription Create(
            ConsumerGroup consumerGroup,
            SubscriptionStartOptions startOptions,
            Func<IReadOnlyCollection<EventDocument>, CancellationToken, Task> changes);
    }
}