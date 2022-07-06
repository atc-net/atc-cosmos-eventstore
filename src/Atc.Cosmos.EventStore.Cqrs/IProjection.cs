using System;
using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Cqrs
{
    public interface IProjection
    {
        Task<ProjectionAction> FailedAsync(
            Exception exception,
            CancellationToken cancellationToken);

        Task InitializeAsync(
            EventStreamId id,
            CancellationToken cancellationToken);

        Task CompleteAsync(
            CancellationToken cancellationToken);
    }
}