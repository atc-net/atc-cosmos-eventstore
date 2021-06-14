using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Streams
{
    public interface IStreamMetadataReader
    {
        Task<IStreamMetadata> GetAsync(StreamId streamId, CancellationToken cancellationToken);
    }
}