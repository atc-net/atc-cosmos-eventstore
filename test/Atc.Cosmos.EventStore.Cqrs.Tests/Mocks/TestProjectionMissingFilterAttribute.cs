using System;
using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Cqrs.Tests.Mocks
{
    internal class TestProjectionMissingFilterAttribute : IProjection
    {
        public Task CompleteAsync(
            CancellationToken cancellationToken)
            => throw new NotImplementedException();

        public Task<ProjectionAction> FailedAsync(
            Exception exception,
            CancellationToken cancellationToken)
            => throw new NotImplementedException();

        public Task InitializeAsync(
            EventStreamId id,
            CancellationToken cancellationToken)
            => throw new NotImplementedException();
    }
}