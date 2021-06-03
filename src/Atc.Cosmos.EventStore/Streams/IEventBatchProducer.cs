using System.Collections.Generic;

namespace Atc.Cosmos.EventStore.Streams
{
    public interface IEventBatchProducer
    {
        StreamBatch FromEvents(
            IReadOnlyCollection<object> events,
            IStreamMetadata metadata,
            StreamWriteOptions? options);
    }
}