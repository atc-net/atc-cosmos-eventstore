using System.Threading.Tasks;
using Atc.Cosmos.EventStore.Diagnostics;
using Microsoft.Azure.Cosmos;

namespace Atc.Cosmos.EventStore.Cosmos
{
    public class CosmosSubscriptionProcessor : IStreamSubscription
    {
        private readonly ISubscriptionTelemetry telemetry;
        private readonly ChangeFeedProcessor processor;
        private readonly ConsumerGroup consumerGroup;
        private ISubscriptionActivity? activity;

        public CosmosSubscriptionProcessor(
            ISubscriptionTelemetry telemetry,
            ChangeFeedProcessor processor,
            ConsumerGroup consumerGroup)
        {
            this.telemetry = telemetry;
            this.processor = processor;
            this.consumerGroup = consumerGroup;
        }

        public async Task StartAsync()
        {
            await processor
                .StartAsync()
                .ConfigureAwait(false);

            activity = telemetry.SubscriptionStarted(consumerGroup);
        }

        public async Task StopAsync()
        {
            await processor
                .StopAsync()
                .ConfigureAwait(false);

            activity?.SubscriptionStopped();
        }
    }
}