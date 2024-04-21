namespace Atc.Cosmos.EventStore.Cqrs.Diagnostics;

public interface IProjectionTelemetry
{
    void ProjectionSkipped(
        string projectionName);

    ProjectionBatchTelemetry StartBatch(
            string projectionName,
            int count);
}