using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Streams
{
    public interface IStreamInfoReader
    {
        ValueTask<IStreamMetadata> ReadAsync(
            StreamId streamId,
            CancellationToken cancellationToken = default);
    }
}