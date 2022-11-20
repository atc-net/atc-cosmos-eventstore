namespace Atc.Cosmos.EventStore.Cqrs;

/// <summary>
/// Implementing this interface will instruct the framework to
/// call <see cref="IConsumeAnyEventAsync.ConsumeAsync(object, EventMetadata, CancellationToken)"/> for every
/// event in the event stream.
/// </summary>
public interface IConsumeAnyEventAsync
{
    Task ConsumeAsync(
        object evt,
        EventMetadata metadata,
        CancellationToken cancellationToken);
}
