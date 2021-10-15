using Atc.Cosmos.EventStore.Streams.Validators;
using Atc.Test;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests.Streams.Validators
{
    public class StreamExpectedVersionValidatorTests
    {
        [Theory, AutoNSubstituteData]
        internal void Should_Validate_When_ExpectedVersion_IsAny(
            IStreamMetadata metadata,
            StreamExpectedVersionValidator sut)
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
        internal void Should_Throw_When_Stream_Exceeds_ExpectedVersion(
            IStreamMetadata metadata,
            StreamExpectedVersionValidator sut)
        {
            metadata
                .State
                .Returns(StreamState.Active);
            metadata
                .Version
                .Returns(StreamVersion.FromStreamVersion(3));

            FluentActions.Invoking(
                () => sut.Validate(metadata, 1))
                .Should()
                .Throw<StreamVersionConflictException>();
        }

        [Theory, AutoNSubstituteData]
        internal void Should_Validate_When_StreamVersion_Is_ExpectedVersion(
            IStreamMetadata metadata,
            StreamExpectedVersionValidator sut)
        {
            metadata
                .State
                .Returns(StreamState.Active);
            metadata
                .Version
                .Returns(StreamVersion.FromStreamVersion(3));

            FluentActions.Invoking(
                () => sut.Validate(metadata, 3))
                .Should()
                .NotThrow();
        }
    }
}