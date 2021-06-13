using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Streams
{
    public interface IStreamInfoReader
    {
        Task<IStreamMetadata> ReadAsync(
            StreamId streamId,
            CancellationToken cancellationToken = default);
    }
}