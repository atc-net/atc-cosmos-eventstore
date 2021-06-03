using System;
using Atc.Cosmos.EventStore.Streams;
using Atc.Test;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests.Streams
{
    public class StreamReadValidatorTests
    {
        [Theory]
        [InlineAutoNSubstituteData(StreamState.Closed, StreamVersion.StartOfStreamValue, 1)]
        [InlineAutoNSubstituteData(StreamState.Closed, StreamVersion.StartOfStreamValue, 10)]
        [InlineAutoNSubstituteData(StreamState.Closed, StreamVersion.StartOfStreamValue, 100)]
        [InlineAutoNSubstituteData(StreamState.Closed, StreamVersion.NotEmptyValue, StreamVersion.StartOfStreamValue)]
        [InlineAutoNSubstituteData(StreamState.Closed, 5, 10)]
        [InlineAutoNSubstituteData(StreamState.Closed, 5, 100)]
        [InlineAutoNSubstituteData(StreamState.New, StreamVersion.StartOfStreamValue, 1)]
        [InlineAutoNSubstituteData(StreamState.New, StreamVersion.StartOfStreamValue, 10)]
        [InlineAutoNSubstituteData(StreamState.New, StreamVersion.StartOfStreamValue, 100)]
        [InlineAutoNSubstituteData(StreamState.New, StreamVersion.NotEmptyValue, StreamVersion.StartOfStreamValue)]
        [InlineAutoNSubstituteData(StreamState.New, 5, 10)]
        [InlineAutoNSubstituteData(StreamState.New, 5, 100)]
        [InlineAutoNSubstituteData(StreamState.Active, StreamVersion.StartOfStreamValue, 1)]
        [InlineAutoNSubstituteData(StreamState.Active, StreamVersion.StartOfStreamValue, 10)]
        [InlineAutoNSubstituteData(StreamState.Active, StreamVersion.StartOfStreamValue, 100)]
        [InlineAutoNSubstituteData(StreamState.Active, StreamVersion.NotEmptyValue, StreamVersion.StartOfStreamValue)]
        [InlineAutoNSubstituteData(StreamState.Active, 5, 10)]
        [InlineAutoNSubstituteData(StreamState.Active, 5, 100)]
        public void Should_Throw_On_Validate(
            StreamState streamState,
            long expectedVersion,
            long actualVersion,
            StreamReadValidator sut,
            IStreamMetadata metadata)
        {
            metadata
                .State
                .Returns(streamState);
            metadata
                .Version
                .Returns(actualVersion);

            FluentActions.Invoking(
                () => sut.Validate(metadata, expectedVersion))
                .Should()
                .Throw<Exception>();
        }

        [Theory]
        [InlineAutoNSubstituteData(StreamState.Closed, StreamVersion.AnyValue, 0)]
        [InlineAutoNSubstituteData(StreamState.Closed, StreamVersion.AnyValue, 10)]
        [InlineAutoNSubstituteData(StreamState.Closed, StreamVersion.AnyValue, 100)]
        [InlineAutoNSubstituteData(StreamState.Closed, StreamVersion.StartOfStreamValue, 0)]
        [InlineAutoNSubstituteData(StreamState.Closed, StreamVersion.NotEmptyValue, 1)]
        [InlineAutoNSubstituteData(StreamState.Closed, StreamVersion.NotEmptyValue, 10)]
        [InlineAutoNSubstituteData(StreamState.Closed, StreamVersion.NotEmptyValue, 100)]
        [InlineAutoNSubstituteData(StreamState.Closed, 5, 5)]
        [InlineAutoNSubstituteData(StreamState.New, StreamVersion.AnyValue, 0)]
        [InlineAutoNSubstituteData(StreamState.New, StreamVersion.AnyValue, 10)]
        [InlineAutoNSubstituteData(StreamState.New, StreamVersion.AnyValue, 100)]
        [InlineAutoNSubstituteData(StreamState.New, StreamVersion.StartOfStreamValue, 0)]
        [InlineAutoNSubstituteData(StreamState.New, StreamVersion.NotEmptyValue, 1)]
        [InlineAutoNSubstituteData(StreamState.New, StreamVersion.NotEmptyValue, 10)]
        [InlineAutoNSubstituteData(StreamState.New, StreamVersion.NotEmptyValue, 100)]
        [InlineAutoNSubstituteData(StreamState.New, 5, 5)]
        [InlineAutoNSubstituteData(StreamState.Active, StreamVersion.AnyValue, 0)]
        [InlineAutoNSubstituteData(StreamState.Active, StreamVersion.AnyValue, 10)]
        [InlineAutoNSubstituteData(StreamState.Active, StreamVersion.AnyValue, 100)]
        [InlineAutoNSubstituteData(StreamState.Active, StreamVersion.StartOfStreamValue, 0)]
        [InlineAutoNSubstituteData(StreamState.Active, StreamVersion.NotEmptyValue, 1)]
        [InlineAutoNSubstituteData(StreamState.Active, StreamVersion.NotEmptyValue, 10)]
        [InlineAutoNSubstituteData(StreamState.Active, StreamVersion.NotEmptyValue, 100)]
        [InlineAutoNSubstituteData(StreamState.Active, 5, 5)]
        public void Should_Validate(
            StreamState streamState,
            long expectedVersion,
            long actualVersion,
            StreamReadValidator sut,
            IStreamMetadata metadata)
        {
            metadata
                .State
                .Returns(streamState);
            metadata
                .Version
                .Returns(actualVersion);

            FluentActions.Invoking(
                () => sut.Validate(metadata, expectedVersion))
                .Should()
                .NotThrow();
        }
    }
}