using Atc.Cosmos.EventStore.Diagnostics;
using Microsoft.Azure.Cosmos;

namespace Atc.Cosmos.EventStore.Cosmos;

internal sealed class CosmosSubscriptionProcessor :
    IStreamSubscription
{
    private readonly ISubscriptionProcessorTelemetry telemetry;
    private readonly ChangeFeedProcessor processor;
    private readonly ConsumerGroup consumerGroup;

    public CosmosSubscriptionProcessor(
        ISubscriptionProcessorTelemetry telemetry,
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

        telemetry.SubscriptionStarted(consumerGroup);
    }

    public async Task StopAsync()
    {
        await processor
            .StopAsync()
            .ConfigureAwait(false);

        telemetry.SubscriptionStopped(consumerGroup);
    }
}