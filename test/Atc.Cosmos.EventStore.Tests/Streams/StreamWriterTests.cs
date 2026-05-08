namespace Atc.Cosmos.EventStore.Tests.Streams;

public sealed class StreamWriterTests
{
    [Theory, AutoNSubstituteData]
    internal async Task Should_Read_Metadata_From_StreamId(
        [Frozen, Substitute] IStreamMetadataReader metadataReader,
        StreamWriter sut,
        StreamId streamId,
        IReadOnlyList<object> events,
        CancellationToken cancellationToken)
    {
        // Act
        await sut.WriteAsync(
            streamId,
            events,
            StreamVersion.Any,
            options: null,
            cancellationToken: cancellationToken);

        // Assert
        _ = metadataReader
            .Received()
            .GetAsync(
                streamId,
                cancellationToken);
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Validate_Metadata_With_Required_Version(
        [Frozen, Substitute] IStreamWriteValidator validator,
        [Frozen, Substitute] IStreamMetadataReader metadataReader,
        StreamWriter sut,
        StreamId streamId,
        IReadOnlyList<object> events,
        StreamMetadata expected,
        CancellationToken cancellationToken)
    {
        // Arrange
        metadataReader
            .GetAsync(default, default)
            .ReturnsForAnyArgs(Task.FromResult<IStreamMetadata>(expected));

        // Act
        await sut.WriteAsync(
            streamId,
            events,
            StreamVersion.Any,
            options: null,
            cancellationToken: cancellationToken);

        // Assert
        validator
            .Received()
            .Validate(
                expected,
                StreamVersion.Any);
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Convert_Events(
        [Frozen, Substitute] IEventBatchProducer eventConverter,
        [Frozen, Substitute] IStreamMetadataReader metadataReader,
        StreamWriter sut,
        StreamId streamId,
        IReadOnlyList<object> events,
        StreamMetadata expected,
        CancellationToken cancellationToken)
    {
        // Arrange
        metadataReader
            .GetAsync(default, default)
            .ReturnsForAnyArgs(Task.FromResult<IStreamMetadata>(expected));

        // Act
        await sut.WriteAsync(
            streamId,
            events,
            StreamVersion.Any,
            options: null,
            cancellationToken: cancellationToken);

        // Assert
        eventConverter
            .Received()
            .FromEvents(
                events,
                expected,
                Arg.Any<StreamWriteOptions>());
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Return_State_From_EventWriter(
        [Frozen, Substitute] IStreamBatchWriter eventWriter,
        StreamWriter sut,
        StreamId streamId,
        IReadOnlyList<object> events,
        StreamMetadata metadata,
        CancellationToken cancellationToken)
    {
        // Arrange
        eventWriter
            .WriteAsync(default, default)
            .ReturnsForAnyArgs(Task.FromResult<IStreamMetadata>(metadata));
        var expected = new StreamResponse(
            metadata.StreamId,
            metadata.Version,
            metadata.Timestamp,
            metadata.State);

        // Act
        var result = await sut.WriteAsync(
            streamId,
            events,
            StreamVersion.Any,
            options: null,
            cancellationToken: cancellationToken);

        // Assert
        result
            .Should()
            .BeEquivalentTo(expected);
    }
}