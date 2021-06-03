using System.Collections.Generic;
using Atc.Cosmos.EventStore.Events;

namespace Atc.Cosmos.EventStore.Streams
{
    public class StreamBatch
    {
        public StreamBatch(
            StreamMetadata metadata,
            IReadOnlyCollection<EventDocument> events)
        {
            Metadata = metadata;
            Documents = events;
        }

        public StreamMetadata Metadata { get; }

        public IReadOnlyCollection<EventDocument> Documents { get; }
    }
}