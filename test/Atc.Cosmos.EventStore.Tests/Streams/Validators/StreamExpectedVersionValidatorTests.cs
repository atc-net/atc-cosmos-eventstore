namespace Atc.Cosmos.EventStore.Tests.Streams.Validators;

public sealed class StreamExpectedVersionValidatorTests
{
    [Theory, AutoNSubstituteData]
    internal void Should_Validate_When_ExpectedVersion_IsAny(
        IStreamMetadata metadata,
        StreamExpectedVersionValidator sut)
    {
        // Arrange
        metadata
            .State
            .Returns(StreamState.Active);

        // Act
        var act = () => sut.Validate(metadata, StreamVersion.Any);

        // Assert
        act.Should().NotThrow();
    }

    [Theory, AutoNSubstituteData]
    internal void Should_Throw_When_Stream_Exceeds_ExpectedVersion(
        IStreamMetadata metadata,
        StreamExpectedVersionValidator sut)
    {
        // Arrange
        metadata
            .State
            .Returns(StreamState.Active);
        metadata
            .Version
            .Returns(StreamVersion.FromStreamVersion(3));

        // Act
        var act = () => sut.Validate(metadata, 1);

        // Assert
        act.Should().Throw<StreamVersionConflictException>();
    }

    [Theory, AutoNSubstituteData]
    internal void Should_Validate_When_StreamVersion_Is_ExpectedVersion(
        IStreamMetadata metadata,
        StreamExpectedVersionValidator sut)
    {
        // Arrange
        metadata
            .State
            .Returns(StreamState.Active);
        metadata
            .Version
            .Returns(StreamVersion.FromStreamVersion(3));

        // Act
        var act = () => sut.Validate(metadata, 3);

        // Assert
        act.Should().NotThrow();
    }
}