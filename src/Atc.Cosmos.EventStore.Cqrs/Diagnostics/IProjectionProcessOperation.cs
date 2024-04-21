namespace Atc.Cosmos.EventStore.Cqrs.Diagnostics;

public interface IProjectionProcessOperationTelemetry : IDisposable
{
    void ProjectionSkipped();

    void ProjectionCompleted();

    void ProjectionFailed(
        Exception exception);
}