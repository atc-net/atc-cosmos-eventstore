using System;
using Microsoft.Azure.Cosmos;

namespace BigBang.Cosmos.EventStore.Readers
{
    public class EventStreamChangeFeedProcessor
    {
        private readonly Container container;
        private readonly Container leaseContainer;

        public EventStreamChangeFeedProcessor(Container container, Container leaseContainer)
        {
            this.container = container;
            this.leaseContainer = leaseContainer;
        }

        public EventStreamSubscription GetSubscription(
            string name,
            string instanceName,
            TimeSpan pollingInterval,
            SubscriptionDelegate callback)
            => new EventStreamSubscription(container, leaseContainer, name, instanceName, pollingInterval, callback);
    }
}