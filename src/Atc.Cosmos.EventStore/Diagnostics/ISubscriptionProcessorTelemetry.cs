namespace Atc.Cosmos.EventStore.Diagnostics;

public interface ISubscriptionProcessorTelemetry
{
    void ProcessExceptionHandlerFailed(
        Exception exception,
        ConsumerGroup consumerGroup);

    void SubscriptionStarted(
        ConsumerGroup consumerGroup);

    void SubscriptionStopped(
        ConsumerGroup consumerGroup);
}