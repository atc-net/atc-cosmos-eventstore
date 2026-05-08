namespace Atc.Cosmos.EventStore.Tests.Streams.Validators;

public sealed class StreamClosedValidatorTests
{
    [Theory, AutoNSubstituteData]
    internal void Should_Validate_When_Stream_IsNew(
        IStreamMetadata metadata,
        StreamVersion version,
        StreamClosedValidator sut)
    {
        // Arrange
        metadata
            .State
            .Returns(StreamState.New);

        // Act
        var act = () => sut.Validate(metadata, version);

        // Assert
        act.Should().NotThrow();
    }

    [Theory, AutoNSubstituteData]
    internal void Should_Validate_When_Stream_IsActive(
        IStreamMetadata metadata,
        StreamVersion version,
        StreamClosedValidator sut)
    {
        // Arrange
        metadata
            .State
            .Returns(StreamState.Active);

        // Act
        var act = () => sut.Validate(metadata, version);

        // Assert
        act.Should().NotThrow();
    }

    [Theory, AutoNSubstituteData]
    internal void Should_Throw_When_Stream_IsClosed(
        IStreamMetadata metadata,
        StreamVersion version,
        StreamClosedValidator sut)
    {
        // Arrange
        metadata
            .State
            .Returns(StreamState.Closed);

        // Act
        var act = () => sut.Validate(metadata, version);

        // Assert
        act.Should().Throw<StreamClosedException>();
    }
}