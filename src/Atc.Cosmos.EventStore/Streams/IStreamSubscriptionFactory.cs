namespace Atc.Cosmos.EventStore.Streams;

internal interface IStreamSubscriptionFactory
{
    IStreamSubscription Create(
        ConsumerGroup consumerGroup,
        ProcessEventsHandler eventsHandler,
        ProcessExceptionHandler exceptionHandler);
}