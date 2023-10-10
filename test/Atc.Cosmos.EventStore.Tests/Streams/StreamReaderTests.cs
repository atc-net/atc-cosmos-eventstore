using Atc.Cosmos.EventStore.Streams;
using Atc.Test;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests.Streams;

public class StreamReaderTests
{
    [Theory, AutoNSubstituteData]
    internal async ValueTask Should_Read_Metadata_From_StreamId(
        [Frozen, Substitute] IStreamMetadataReader metadataReader,
        [Frozen, Substitute] IStreamIterator streamIterator,
        [Substitute] IAsyncEnumerable<IEvent> enumerable,
        StreamReader sut,
        StreamId streamId,
        CancellationToken cancellationToken)
    {
        streamIterator
            .ReadAsync(default, default, default, default)
            .ReturnsForAnyArgs(enumerable);

        await ReadStream(
            sut,
            streamId,
            StreamVersion.Any,
            filter: null,
            cancellationToken: cancellationToken);

        await metadataReader
            .Received()
            .GetAsync(
                streamId,
                cancellationToken);
    }

    [Theory]
    [InlineAutoNSubstituteData(StreamVersion.NotEmptyValue)]
    [InlineAutoNSubstituteData(StreamVersion.StartOfStreamValue)]
    [InlineAutoNSubstituteData(StreamVersion.AnyValue)]
    internal async Task Should_Validate_Metadata_With_From_Version(
        StreamVersion fromVersion,
        [Frozen, Substitute] IStreamMetadataReader metadataReader,
        [Frozen, Substitute] IStreamReadValidator validator,
        [Frozen, Substitute] IStreamIterator streamIterator,
        [Substitute] IAsyncEnumerable<IEvent> enumerable,
        StreamReader sut,
        StreamId streamId,
        StreamMetadata streamMetadata,
        CancellationToken cancellationToken)
    {
        metadataReader
            .GetAsync(default, default)
            .ReturnsForAnyArgs(Task.FromResult<IStreamMetadata>(streamMetadata));

        streamIterator
            .ReadAsync(default, default, default, default)
            .ReturnsForAnyArgs(enumerable);

        await ReadStream(
            sut,
            streamId,
            fromVersion,
            filter: null,
            cancellationToken: cancellationToken);

        validator
            .Received()
            .Validate(
                streamMetadata,
                fromVersion);
    }

    [Theory, AutoNSubstituteData]
    internal async ValueTask Should_Read_From_Iterator(
        [Frozen, Substitute] IStreamMetadataReader metadataReader,
        [Frozen, Substitute] IStreamIterator streamIterator,
        [Substitute] IAsyncEnumerable<IEvent> enumerable,
        StreamReader sut,
        StreamId streamId,
        StreamReadFilter filter,
        StreamMetadata streamMetadata,
        CancellationToken cancellationToken)
    {
        metadataReader
            .GetAsync(default, default)
            .ReturnsForAnyArgs(Task.FromResult<IStreamMetadata>(streamMetadata));

        streamIterator
            .ReadAsync(default, default, default, default)
            .ReturnsForAnyArgs(enumerable);

        await ReadStream(
            sut,
            streamId,
            StreamVersion.Any,
            filter: filter,
            cancellationToken: cancellationToken);

        streamIterator
            .Received(1)
            .ReadAsync(
                streamId,
                StreamVersion.Any,
                filter,
                cancellationToken);
    }

    [Theory, AutoNSubstituteData]
    internal async ValueTask Should_Yield_Events_From_StreamIterator(
        [Frozen, Substitute] IStreamMetadataReader metadataReader,
        [Frozen, Substitute] IStreamIterator streamIterator,
        [Substitute] IAsyncEnumerable<IEvent> enumerable,
        [Substitute] IAsyncEnumerator<IEvent> enumerator,
        StreamReader sut,
        StreamId streamId,
        StreamMetadata streamMetadata,
        IEvent firstEvent,
        IEvent secondEvent,
        CancellationToken cancellationToken)
    {
        metadataReader
            .GetAsync(default, default)
            .ReturnsForAnyArgs(Task.FromResult<IStreamMetadata>(streamMetadata));

        streamIterator
            .ReadAsync(default, default, default, default)
            .ReturnsForAnyArgs(enumerable);

        enumerable
            .GetAsyncEnumerator(default)
            .ReturnsForAnyArgs(enumerator);

        enumerator
            .Current
            .ReturnsForAnyArgs(firstEvent, secondEvent);

#pragma warning disable CA2012 // Use ValueTasks correctly
        enumerator
            .MoveNextAsync()
#pragma warning restore CA2012 // Use ValueTasks correctly
            .ReturnsForAnyArgs(new ValueTask<bool>(true), new ValueTask<bool>(true), new ValueTask<bool>(false));

        // Act
        var events = await ReadStream(
            sut,
            streamId,
            StreamVersion.Any,
            filter: null,
            cancellationToken: cancellationToken);

        // Assert
        events
            .Should()
            .Contain(firstEvent);
        events
            .Should()
            .Contain(secondEvent);
    }

    private static async Task<List<IEvent>> ReadStream(
        StreamReader sut,
        StreamId streamId,
        StreamVersion streamVersion,
        StreamReadFilter filter,
        CancellationToken cancellationToken)
    {
        var received = new List<IEvent>();
        await foreach (var item in sut.ReadAsync(streamId, streamVersion, filter, cancellationToken).ConfigureAwait(false))
        {
            received.Add(item);
        }

        return received;
    }
}