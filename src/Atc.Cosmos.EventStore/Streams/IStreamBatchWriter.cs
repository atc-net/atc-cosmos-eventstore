using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Streams
{
    public interface IStreamBatchWriter
    {
        ValueTask<IStreamMetadata> WriteAsync(
            StreamBatch batch,
            CancellationToken cancellationToken);
    }
}