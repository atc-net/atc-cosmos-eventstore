using System.Collections.Generic;

namespace BigBang.Cosmos.EventStore
{
    public class PartitionedEvents
    {
        public PartitionedEvents(string streamId, IReadOnlyCollection<Event> events)
        {
            StreamId = streamId;
            Events = events;
        }

        public string StreamId { get; }
        public IReadOnlyCollection<Event> Events { get; }
    }
}