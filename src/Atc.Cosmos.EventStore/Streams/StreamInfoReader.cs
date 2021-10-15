using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Streams
{
    internal class StreamInfoReader : IStreamInfoReader
    {
        private readonly IStreamMetadataReader metadataReader;

        public StreamInfoReader(
            IStreamMetadataReader metadataReader)
            => this.metadataReader = metadataReader;

        public Task<IStreamMetadata> ReadAsync(
            StreamId streamId,
            CancellationToken cancellationToken = default)
            => metadataReader.GetAsync(streamId, cancellationToken);
    }
}