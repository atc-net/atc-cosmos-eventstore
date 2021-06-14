using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Streams
{
    public interface IStreamBatchWriter
    {
        Task<IStreamMetadata> WriteAsync(
            StreamBatch batch,
            CancellationToken cancellationToken);
    }
}