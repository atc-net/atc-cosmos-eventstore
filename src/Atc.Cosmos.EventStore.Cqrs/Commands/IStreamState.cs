namespace Atc.Cosmos.EventStore.Cqrs.Commands
{
    internal interface IStreamState
    {
        StreamId Id { get; }

        StreamVersion Version { get; }
    }
}