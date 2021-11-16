using System.Collections.Generic;

namespace Atc.Cosmos.EventStore.Streams
{
    internal interface IEventBatchProducer
    {
        StreamBatch FromEvents(
            IReadOnlyCollection<object> events,
            IStreamMetadata metadata,
            StreamWriteOptions? options);
    }
}