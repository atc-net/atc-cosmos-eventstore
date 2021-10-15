using Atc.Cosmos.EventStore.Streams.Validators;
using Atc.Test;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests.Streams.Validators
{
    public class StreamNotEmptyValidatorValidatorTests
    {
        [Theory]
        [InlineAutoNSubstituteData(1, StreamState.Active)]
        [InlineAutoNSubstituteData(10, StreamState.Active)]
        [InlineAutoNSubstituteData(StreamVersion.AnyValue, StreamState.Active)]
        [InlineAutoNSubstituteData(1, StreamState.Closed)]
        [InlineAutoNSubstituteData(10, StreamState.Closed)]
        [InlineAutoNSubstituteData(StreamVersion.AnyValue, StreamState.Closed)]
        [InlineAutoNSubstituteData(1, StreamState.New)]
        [InlineAutoNSubstituteData(10, StreamState.New)]
        [InlineAutoNSubstituteData(StreamVersion.AnyValue, StreamState.New)]
        internal void Should_Validate(
            long version,
            StreamState state,
            IStreamMetadata metadata,
            StreamNotEmptyValidator sut)
        {
            metadata
                .State
                .Returns(state);
            metadata
                .Version
                .Returns(version);

            FluentActions.Invoking(
                () => sut.Validate(metadata, StreamVersion.NotEmptyValue))
                .Should()
                .NotThrow();
        }

        [Theory]
        [InlineAutoNSubstituteData(StreamVersion.StartOfStreamValue, StreamState.Active)]
        [InlineAutoNSubstituteData(StreamVersion.StartOfStreamValue, StreamState.Closed)]
        [InlineAutoNSubstituteData(StreamVersion.StartOfStreamValue, StreamState.New)]
        internal void Should_Throw_When_StreamVersion_Is_Not_StartOfStream(
            long version,
            StreamState state,
            IStreamMetadata metadata,
            StreamNotEmptyValidator sut)
        {
            metadata
                .State
                .Returns(state);
            metadata
                .Version
                .Returns(version);

            FluentActions.Invoking(
                () => sut.Validate(metadata, StreamVersion.NotEmptyValue))
                .Should()
                .Throw<StreamVersionConflictException>();
        }
    }
}