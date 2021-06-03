using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Atc.Cosmos.EventStore.Streams;
using Atc.Test;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests.Streams
{
    public class StreamReaderTests
    {
        [Theory, AutoNSubstituteData]
        public async ValueTask Should_Read_Metadata_From_StreamId(
            [Frozen, Substitute] IStreamMetadataReader metadataReader,
            [Frozen, Substitute] IStreamIterator streamIterator,
            [Substitute] IAsyncEnumerable<IEvent> enumerable,
            StreamReader sut,
            StreamId streamId,
            CancellationToken cancellationToken)
        {
            streamIterator
                .ReadAsync(default, default, default)
                .ReturnsForAnyArgs(enumerable);

            await ReadStream(
                sut,
                streamId,
                StreamVersion.Any,
                cancellationToken: cancellationToken);

            await metadataReader
                .Received()
                .GetAsync(
                    streamId,
                    cancellationToken);
        }

        [Theory, AutoNSubstituteData]
        public async ValueTask Should_Validate_Metadata_With_From_Version(
            [Frozen, Substitute] IStreamMetadataReader metadataReader,
            [Frozen, Substitute] IStreamWriteValidator validator,
            [Frozen, Substitute] IStreamIterator streamIterator,
            [Substitute] IAsyncEnumerable<IEvent> enumerable,
            StreamReader sut,
            StreamId streamId,
            StreamMetadata streamMetadata,
            CancellationToken cancellationToken)
        {
            metadataReader
                .GetAsync(default, default)
                .ReturnsForAnyArgs(new ValueTask<IStreamMetadata>(streamMetadata));

            streamIterator
                .ReadAsync(default, default, default)
                .ReturnsForAnyArgs(enumerable);

            await ReadStream(
                sut,
                streamId,
                StreamVersion.Any,
                cancellationToken: cancellationToken);

            validator
                .Received()
                .Validate(
                    streamMetadata,
                    StreamVersion.Any);
        }

        [Theory, AutoNSubstituteData]
        public async ValueTask Should_Yield_Events_From_StreamIterator(
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
                .ReturnsForAnyArgs(new ValueTask<IStreamMetadata>(streamMetadata));

            streamIterator
                .ReadAsync(default, default, default)
                .ReturnsForAnyArgs(enumerable);

            enumerable
                .GetAsyncEnumerator(default)
                .ReturnsForAnyArgs(enumerator);

            enumerator
                .Current
                .ReturnsForAnyArgs(firstEvent, secondEvent);

            enumerator
                .MoveNextAsync()
                .ReturnsForAnyArgs(new ValueTask<bool>(true), new ValueTask<bool>(true), new ValueTask<bool>(false));

            // Act
            var events = await ReadStream(
                sut,
                streamId,
                StreamVersion.Any,
                cancellationToken: cancellationToken);

            // Assert
            events
                .Should()
                .Contain(firstEvent);
            events
                .Should()
                .Contain(secondEvent);
        }

        private static async Task<List<IEvent>> ReadStream(StreamReader sut, StreamId streamId, StreamVersion streamVersion, CancellationToken cancellationToken)
        {
            var received = new List<IEvent>();
            await foreach (var item in sut.ReadAsync(streamId, streamVersion, cancellationToken).ConfigureAwait(false))
            {
                received.Add(item);
            }

            return received;
        }
    }
}