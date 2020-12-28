using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public Task ResumeAsync()
            => StartAsync(true);

        /// <summary>
        /// Starts the subscription from the beginning.
        /// </summary>
        /// <remarks>This will delete all existing subscriptions with the same name for all instances.</remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task StartFromBeginningAsync()
            => StartAsync(resume: false);

        public async Task StopAsync()
        {
            if (processor != null)
            {
                await processor
                    .StopAsync()
                    .ConfigureAwait(false);
            }
        }

        public async Task DeleteAsync()
        {
            if (processor != null)
            {
                await StopAsync()
                    .ConfigureAwait(false);
            }

            await DeleteRegistrationAsync(name)
                .ConfigureAwait(false);
        }

        private async Task ProcessChangesAsync(IReadOnlyCollection<Event> changes, CancellationToken cancellationToken)
        {
            try
            {
                await callback(changes, cancellationToken)
                    .ConfigureAwait(false);
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
                await DeleteRegistrationAsync(name)
                    .ConfigureAwait(false);
            }

            processor = builder.Build();

            await processor
                .StartAsync()
                .ConfigureAwait(false);
        }

        private Task DeleteRegistrationAsync(string name)
        {
            var store = new ChangeFeedLeaseStore(leaseContainer);

            return store.RemoveLeasesAsync(name);
        }
    }
}