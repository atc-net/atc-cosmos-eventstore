namespace Atc.Cosmos.EventStore.Cqrs.Diagnostics;

public interface IProjectionBatchTelemetry : IDisposable
{
    IProjectionProcessOperationTelemetry StartProjection(
        StreamId streamId);

    void BatchCompleted();

    void BatchFailed(
        Exception exception);
}
