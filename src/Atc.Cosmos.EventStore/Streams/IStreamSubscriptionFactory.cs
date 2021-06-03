namespace Atc.Cosmos.EventStore.Streams
{
    public interface IStreamSubscriptionFactory
    {
        IStreamSubscription Create(
            ConsumerGroup consumerGroup,
            SubscriptionStartOptions startOptions,
            ProcessEventsHandler eventsHandler,
            ProcessExceptionHandler errorHandler);
    }
}