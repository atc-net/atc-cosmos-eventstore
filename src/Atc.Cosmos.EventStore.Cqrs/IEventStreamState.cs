namespace Atc.Cosmos.EventStore.Cqrs
{
    public interface IEventStreamState
    {
        EventStreamId Id { get; }

        EventStreamVersion Version { get; }
    }
}