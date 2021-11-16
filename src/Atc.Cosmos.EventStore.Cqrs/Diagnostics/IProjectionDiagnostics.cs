using System.Collections.Generic;
using System.Linq;

namespace Atc.Cosmos.EventStore.Cqrs.Diagnostics
{
    internal interface IProjectionDiagnostics
    {
        void ProcessingGroupedEvents(
            string projectionName,
            IGrouping<StreamId, IEvent>[] groupedEvents,
            IEnumerable<IEvent> batch);

        IProjectionProcessOperation StartStreamProjection(
            string projectionName,
            StreamId key);
    }
}