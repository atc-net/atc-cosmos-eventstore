namespace Atc.Cosmos.EventStore.Streams;

internal interface IStreamSubscriptionRemover
{
    Task DeleteAsync(
        ConsumerGroup consumerGroup,
        CancellationToken cancellationToken);
}