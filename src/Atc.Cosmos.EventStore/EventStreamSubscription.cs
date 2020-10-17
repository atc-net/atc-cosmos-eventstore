using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using BigBang.Cosmos.EventStore.Readers;
using Microsoft.Azure.Cosmos;

namespace BigBang.Cosmos.EventStore
{
    public sealed class EventStreamSubscription
    {
        private readonly Container container;
        private readonly Container leaseContainer;
        private readonly string name;
        private readonly string instanceName;
        private readonly TimeSpan pollingInterval;
        private readonly SubscriptionDelegate callback;
        private ChangeFeedProcessor? processor;

        public EventStreamSubscription(
            Container container,
            Container leaseContainer,
            string name,
            string instanceName,
            TimeSpan pollingInterval,
            SubscriptionDelegate callback)
        {
            this.container = container;
            this.leaseContainer = leaseContainer;
            this.name = name + ":"; // we use this to identify the lease entry
            this.instanceName = instanceName;
            this.pollingInterval = pollingInterval;
            this.callback = callback;
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "ByDesign")]
        private async Task ProcessChangesAsync(IReadOnlyCollection<Event> changes, CancellationToken cancellationToken)
        {
            try
            {
                await callback(changes, cancellationToken);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private async Task StartAsync(bool resume)
        {
            var builder = container.GetChangeFeedProcessorBuilder<Event>(name, ProcessChangesAsync)
                .WithInstanceName(instanceName)
                .WithPollInterval(pollingInterval)
                .WithStartTime(DateTime.MinValue.ToUniversalTime())
                .WithLeaseContainer(leaseContainer);

            if (!resume)
            {
                await DeleteRegistrationAsync(name);
            }

            processor = builder.Build();

            await processor.StartAsync();
        }

        public Task ResumeAsync()
            => StartAsync(true);

        /// <summary>
        /// Starts the subscription from the beginning.
        /// </summary>
        /// <remarks>This will delete all existing subscriptions with the same name for all instances.</remarks>
        public Task StartFromBeginningAsync()
            => StartAsync(false);

        public async Task StopAsync()
        {
            if (processor != null)
            {
                await processor.StopAsync();
            }
        }

        public async Task DeleteAsync()
        {
            if (processor != null)
            {
                await StopAsync();
            }

            await DeleteRegistrationAsync(name);
        }

        private Task DeleteRegistrationAsync(string name)
        {
            var store = new ChangeFeedLeaseStore(leaseContainer);

            return store.RemoveLeasesAsync(name);
        }
    }
}