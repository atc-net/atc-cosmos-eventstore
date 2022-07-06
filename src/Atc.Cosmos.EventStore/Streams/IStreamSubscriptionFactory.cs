namespace Atc.Cosmos.EventStore.Streams
{
    internal interface IStreamSubscriptionFactory
    {
        IStreamSubscription Create(
            ConsumerGroup consumerGroup,
            SubscriptionStartOptions startOptions,
            ProcessEventsHandler eventsHandler,
            ProcessExceptionHandler exceptionHandler);
    }
}