using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Cqrs
{
    public interface IProjection
    {
        Task InitializeAsync(
            EventStreamId id,
            CancellationToken cancellationToken);

        Task CompleteAsync(
            CancellationToken cancellationToken);
    }
}