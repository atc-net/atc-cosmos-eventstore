namespace Atc.Cosmos.EventStore.Cqrs.Commands;

internal class StreamState : IStreamState
{
    public StreamId Id { get; set; }

    public StreamVersion Version { get; set; }
}