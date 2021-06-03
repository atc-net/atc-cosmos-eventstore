using System.Threading;
using System.Threading.Tasks;
using Atc.Cosmos.EventStore.Streams;
using Atc.Test;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests.Streams
{
    public class StreamInfoReaderTests
    {
        [Theory, AutoNSubstituteData]
        public async Task Should_Convert_Into_StreamResponse(
            [Frozen, Substitute] IStreamMetadataReader metadataReader,
            StreamInfoReader sut,
            StreamId streamId,
            StreamMetadata expectedMetadata,
            CancellationToken cancellationToken)
        {
            metadataReader
                .GetAsync(default, cancellationToken)
                .ReturnsForAnyArgs(expectedMetadata);

            var info = await sut
                .ReadAsync(streamId, cancellationToken)
                .ConfigureAwait(false);

            info
                .State
                .Should()
                .Be(expectedMetadata.State);
            info
                .StreamId
                .Should()
                .Be(expectedMetadata.StreamId);
            info
                .Timestamp
                .Should()
                .Be(expectedMetadata.Timestamp);
            info
                .Version
                .Should()
                .Be(expectedMetadata.Version);
        }

        [Theory, AutoNSubstituteData]
        public async Task Should_Read_Metadata_From_Stream(
            [Frozen, Substitute] IStreamMetadataReader metadataReader,
            StreamInfoReader sut,
            StreamId streamId,
            StreamMetadata expectedMetadata,
            CancellationToken cancellationToken)
        {
            metadataReader
                .GetAsync(default, cancellationToken)
                .ReturnsForAnyArgs(expectedMetadata);

            await sut
                .ReadAsync(streamId, cancellationToken)
                .ConfigureAwait(false);

            _ = metadataReader
                    .Received(1)
                    .GetAsync(
                        streamId,
                        cancellationToken);
        }
    }
}