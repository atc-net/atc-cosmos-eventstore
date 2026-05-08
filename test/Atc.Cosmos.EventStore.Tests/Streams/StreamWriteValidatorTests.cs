namespace Atc.Cosmos.EventStore.Tests.Streams;

public sealed class StreamWriteValidatorTests
{
    [Theory]
    [InlineAutoNSubstituteData(StreamState.Closed, 1, 1)]
    [InlineAutoNSubstituteData(StreamState.Closed, 0, 1)]
    [InlineAutoNSubstituteData(StreamState.Closed, 10, 1)]
    [InlineAutoNSubstituteData(StreamState.New, 0, 1)]
    [InlineAutoNSubstituteData(StreamState.New, 0, 10)]
    [InlineAutoNSubstituteData(StreamState.Active, 1, 2)]
    [InlineAutoNSubstituteData(StreamState.Active, 10, 2)]
    internal void Should_Throw_On_Validate(
        StreamState streamState,
        long streamVersion,
        long expectedVersion,
        StreamWriteValidator sut,
        IStreamMetadata metadata)
    {
        // Arrange
        metadata
            .State
            .Returns(streamState);
        metadata
            .Version
            .Returns(streamVersion);

        // Act
        var act = () => sut.Validate(metadata, expectedVersion);

        // Assert
        act.Should().Throw<Exception>();
    }

    [Theory]
    [InlineAutoNSubstituteData(StreamState.New, 0, StreamVersion.AnyValue)]
    [InlineAutoNSubstituteData(StreamState.New, 0, StreamVersion.StartOfStreamValue)]
    [InlineAutoNSubstituteData(StreamState.Active, 1, 1)]
    [InlineAutoNSubstituteData(StreamState.Active, 10, 10)]
    [InlineAutoNSubstituteData(StreamState.Active, 10, StreamVersion.AnyValue)]
    internal void Should_Validate(
        StreamState streamState,
        long streamVersion,
        long expectedVersion,
        StreamWriteValidator sut,
        IStreamMetadata metadata)
    {
        // Arrange
        metadata
            .State
            .Returns(streamState);
        metadata
            .Version
            .Returns(streamVersion);

        // Act
        var act = () => sut.Validate(metadata, expectedVersion);

        // Assert
        act.Should().NotThrow();
    }
}