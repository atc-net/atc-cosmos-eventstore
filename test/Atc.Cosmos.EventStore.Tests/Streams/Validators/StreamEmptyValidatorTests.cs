using Atc.Cosmos.EventStore.Streams.Validators;
using Atc.Test;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests.Streams.Validators;

public class StreamEmptyValidatorTests
{
    [Theory, AutoNSubstituteData]
    internal void Should_Validate_When_Expected_Version_IsNot_StartOfStream(
        IStreamMetadata metadata,
        StreamEmptyValidator sut)
    {
        metadata
            .State
            .Returns(StreamState.Active);

        FluentActions.Invoking(
            () => sut.Validate(metadata, StreamVersion.Any))
            .Should()
            .NotThrow();
    }

    [Theory, AutoNSubstituteData]
    internal void Should_Throw_When_Stream_IsRequired_ToBe_StartOfStream_But_Stream_IsNot_StartOfStream(
        IStreamMetadata metadata,
        StreamEmptyValidator sut)
    {
        metadata
            .State
            .Returns(StreamState.Active);
        metadata
            .Version
            .Returns(StreamVersion.FromStreamVersion(1));

        FluentActions.Invoking(
            () => sut.Validate(metadata, StreamVersion.StartOfStream))
            .Should()
            .Throw<StreamVersionConflictException>();
    }

    [Theory, AutoNSubstituteData]
    internal void Should_Validate_When_Stream_Is_StartOfStream(
        IStreamMetadata metadata,
        StreamEmptyValidator sut)
    {
        metadata
            .State
            .Returns(StreamState.Active);
        metadata
            .Version
            .Returns(StreamVersion.StartOfStream);

        FluentActions.Invoking(
            () => sut.Validate(metadata, StreamVersion.StartOfStream))
            .Should()
            .NotThrow();
    }
}