using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Streams
{
    internal class StreamReader : IStreamReader
    {
        private readonly IStreamMetadataReader metadataReader;
        private readonly IStreamReadValidator readValidator;
        private readonly IStreamIterator streamIterator;

        public StreamReader(
            IStreamMetadataReader metadataReader,
            IStreamReadValidator readValidator,
            IStreamIterator streamIterator)
        {
            this.metadataReader = metadataReader;
            this.readValidator = readValidator;
            this.streamIterator = streamIterator;
        }

        public async IAsyncEnumerable<IEvent> ReadAsync(
            StreamId streamId,
            StreamVersion fromVersion,
            StreamReadFilter? filter,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var metadata = await metadataReader
                .GetAsync(streamId, cancellationToken)
                .ConfigureAwait(false);

            readValidator.Validate(
                metadata,
                filter?.RequiredVersion ?? StreamVersion.Any);

            // If we don't have any events in the stream, then skip reading from stream.
            if (metadata.Version == 0)
            {
                yield break;
            }

            await foreach (var evt in streamIterator
                .ReadAsync(streamId, fromVersion, filter, cancellationToken)
                .ConfigureAwait(false))
            {
                yield return evt;
            }
        }
    }
}