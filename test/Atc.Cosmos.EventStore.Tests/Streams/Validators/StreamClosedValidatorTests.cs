using Atc.Cosmos.EventStore.Streams.Validators;
using Atc.Test;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests.Streams.Validators
{
    public class StreamClosedValidatorTests
    {
        [Theory, AutoNSubstituteData]
        internal void Should_Validate_When_Stream_IsNew(
            IStreamMetadata metadata,
            StreamVersion version,
            StreamClosedValidator sut)
        {
            metadata
                .State
                .Returns(StreamState.New);

            FluentActions.Invoking(
                () => sut.Validate(metadata, version))
                .Should()
                .NotThrow();
        }

        [Theory, AutoNSubstituteData]
        internal void Should_Validate_When_Stream_IsActive(
            IStreamMetadata metadata,
            StreamVersion version,
            StreamClosedValidator sut)
        {
            metadata
                .State
                .Returns(StreamState.Active);

            FluentActions.Invoking(
                () => sut.Validate(metadata, version))
                .Should()
                .NotThrow();
        }

        [Theory, AutoNSubstituteData]
        internal void Should_Throw_When_Stream_IsClosed(
            IStreamMetadata metadata,
            StreamVersion version,
            StreamClosedValidator sut)
        {
            metadata
                .State
                .Returns(StreamState.Closed);

            FluentActions.Invoking(
                () => sut.Validate(metadata, version))
                .Should()
                .Throw<StreamClosedException>();
        }
    }
}