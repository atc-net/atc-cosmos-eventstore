namespace Atc.Cosmos.EventStore.Cqrs.Commands
{
    internal class StreamState : IEventStreamState
    {
        public StreamState(
            EventStreamId streamId)
        {
            Id = streamId;
            Version = 0L;
        }

        public EventStreamId Id { get; set; }

        public EventStreamVersion Version { get; set; }
    }
}