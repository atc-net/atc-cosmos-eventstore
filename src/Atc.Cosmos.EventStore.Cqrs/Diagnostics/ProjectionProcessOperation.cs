using System;
using System.Linq;

namespace Atc.Cosmos.EventStore.Cqrs.Diagnostics
{
    internal class ProjectionProcessOperation : IProjectionProcessOperation
    {
        public void ProjectionCompleted(
            IGrouping<StreamId, IEvent> events)
        {
            // Ignore
        }

        public void ProjectionFailed(
            IGrouping<StreamId, IEvent> events,
            Exception exception)
        {
            // Ignore
        }

        public void ProjectionSkipped(
            IGrouping<StreamId, IEvent> events)
        {
            // Ignore
        }
    }
}