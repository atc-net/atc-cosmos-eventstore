namespace Atc.Cosmos.EventStore.Tests.Streams.Validators;

public sealed class StreamEmptyValidatorTests
{
    [Theory, AutoNSubstituteData]
    internal void Should_Validate_When_Expected_Version_IsNot_StartOfStream(
        IStreamMetadata metadata,
        StreamEmptyValidator sut)
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
    internal void Should_Throw_When_Stream_IsRequired_ToBe_StartOfStream_But_Stream_IsNot_StartOfStream(
        IStreamMetadata metadata,
        StreamEmptyValidator sut)
    {
        // Arrange
        metadata
            .State
            .Returns(StreamState.Active);
        metadata
            .Version
            .Returns(StreamVersion.FromStreamVersion(1));

        // Act
        var act = () => sut.Validate(metadata, StreamVersion.StartOfStream);

        // Assert
        act.Should().Throw<StreamVersionConflictException>();
    }

    [Theory, AutoNSubstituteData]
    internal void Should_Validate_When_Stream_Is_StartOfStream(
        IStreamMetadata metadata,
        StreamEmptyValidator sut)
    {
        // Arrange
        metadata
            .State
            .Returns(StreamState.Active);
        metadata
            .Version
            .Returns(StreamVersion.StartOfStream);

        // Act
        var act = () => sut.Validate(metadata, StreamVersion.StartOfStream);

        // Assert
        act.Should().NotThrow();
    }
}