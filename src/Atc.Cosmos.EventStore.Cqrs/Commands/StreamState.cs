namespace Atc.Cosmos.EventStore.Cqrs.Commands
{
    public class StreamState : IStreamState
    {
        public StreamId Id { get; set; }

        public StreamVersion Version { get; set; }
    }
}