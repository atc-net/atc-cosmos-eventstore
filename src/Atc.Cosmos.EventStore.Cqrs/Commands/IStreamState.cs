namespace Atc.Cosmos.EventStore.Cqrs.Commands
{
    public interface IStreamState
    {
        StreamId Id { get; }

        StreamVersion Version { get; }
    }
}