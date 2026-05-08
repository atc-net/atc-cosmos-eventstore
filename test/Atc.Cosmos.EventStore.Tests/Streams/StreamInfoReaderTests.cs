namespace Atc.Cosmos.EventStore.Tests.Streams;

public sealed class StreamInfoReaderTests
{
    [Theory, AutoNSubstituteData]
    internal async Task Should_Convert_Into_StreamResponse(
        [Frozen, Substitute] IStreamMetadataReader metadataReader,
        StreamInfoReader sut,
        StreamId streamId,
        StreamMetadata expectedMetadata,
        CancellationToken cancellationToken)
    {
        // Arrange
        metadataReader
            .GetAsync(default, cancellationToken)
            .ReturnsForAnyArgs(expectedMetadata);

        // Act
        var info = await sut
            .ReadAsync(streamId, cancellationToken);

        // Assert
        info.State.Should().Be(expectedMetadata.State);
        info.StreamId.Should().Be(expectedMetadata.StreamId);
        info.Timestamp.Should().Be(expectedMetadata.Timestamp);
        info.Version.Should().Be(expectedMetadata.Version);
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Read_Metadata_From_Stream(
        [Frozen, Substitute] IStreamMetadataReader metadataReader,
        StreamInfoReader sut,
        StreamId streamId,
        StreamMetadata expectedMetadata,
        CancellationToken cancellationToken)
    {
        // Arrange
        metadataReader
            .GetAsync(default, cancellationToken)
            .ReturnsForAnyArgs(expectedMetadata);

        // Act
        await sut
            .ReadAsync(streamId, cancellationToken);

        // Assert
        _ = metadataReader
                .Received(1)
                .GetAsync(
                    streamId,
                    cancellationToken);
    }
}