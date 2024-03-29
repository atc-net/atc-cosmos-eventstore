namespace Atc.Cosmos.EventStore.Cqrs.Projections;

public interface IProjectionMetadata
{
    bool CanConsumeEvent(
        IEvent evt);

    bool CanConsumeOneOrMoreEvents(
        IEnumerable<IEvent> events);

    bool IsNotConsumingEvents();

    ValueTask ConsumeEventsAsync(
        IEnumerable<IEvent> events,
        IProjection projection,
        CancellationToken cancellationToken);
}