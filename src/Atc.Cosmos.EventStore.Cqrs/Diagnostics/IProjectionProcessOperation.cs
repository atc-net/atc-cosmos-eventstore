namespace Atc.Cosmos.EventStore.Cqrs.Diagnostics;

internal interface IProjectionProcessOperation
{
    void ProjectionSkipped(
        IGrouping<StreamId, IEvent> events);

    void ProjectionCompleted(
        IGrouping<StreamId, IEvent> events);

    void ProjectionFailed(
        IGrouping<StreamId, IEvent> events,
        Exception exception);
}