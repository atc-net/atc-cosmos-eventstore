namespace Atc.Cosmos.EventStore.Cqrs;

public interface IAggregateProjection
{
    ValueTask InitializeAsync(
        CancellationToken cancellationToken);

    ValueTask CompleteAsync(
        CancellationToken cancellationToken);
}
