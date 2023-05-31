namespace Atc.Cosmos.EventStore.Diagnostics;

internal interface ISubscriptionTelemetry
{
    void ProcessExceptionHandlerFailed(Exception exception, ConsumerGroup consumerGroup);

    ISubscriptionActivity? SubscriptionStarted(ConsumerGroup consumerGroup);
}