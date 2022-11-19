using Atc.Cosmos.EventStore.Streams;
using Atc.Test;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests.Streams;

public class StreamWriterTests
{
    [Theory, AutoNSubstituteData]
    internal async ValueTask Should_Read_Metadata_From_StreamId(
        [Frozen, Substitute] IStreamMetadataReader metadataReader,
        [Frozen, Substitute] IStreamBatchWriter eventWriter,
        StreamWriter sut,
        StreamId streamId,
        IReadOnlyList<object> events,
        StreamMetadata expected,
        CancellationToken cancellationToken)
    {
        eventWriter
            .WriteAsync(default, default)
            .ReturnsForAnyArgs(Task.FromResult<IStreamMetadata>(expected));

        await sut.WriteAsync(
            streamId,
            events,
            StreamVersion.Any,
            options: null,
            cancellationToken: cancellationToken);

        _ = metadataReader
            .Received()
            .GetAsync(
                streamId,
                cancellationToken);
    }

    [Theory, AutoNSubstituteData]
    internal async ValueTask Should_Validate_Metadata_With_Required_Version(
        [Frozen, Substitute] IStreamWriteValidator validator,
        [Frozen, Substitute] IStreamBatchWriter eventWriter,
        StreamWriter sut,
        StreamId streamId,
        IReadOnlyList<object> events,
        StreamMetadata expected,
        CancellationToken cancellationToken)
    {
        eventWriter
            .WriteAsync(default, default)
            .ReturnsForAnyArgs(Task.FromResult<IStreamMetadata>(expected));

        await sut.WriteAsync(
            streamId,
            events,
            StreamVersion.Any,
            options: null,
            cancellationToken: cancellationToken);

        validator
            .Received()
            .Validate(
                expected,
                StreamVersion.Any);
    }

    [Theory, AutoNSubstituteData]
    internal async ValueTask Should_Convert_Events(
        [Frozen, Substitute] IEventBatchProducer eventConverter,
        [Frozen, Substitute] IStreamBatchWriter eventWriter,
        StreamWriter sut,
        StreamId streamId,
        IReadOnlyList<object> events,
        StreamMetadata expected,
        CancellationToken cancellationToken)
    {
        eventWriter
            .WriteAsync(default, default)
            .ReturnsForAnyArgs(Task.FromResult<IStreamMetadata>(expected));

        await sut.WriteAsync(
            streamId,
            events,
            StreamVersion.Any,
            options: null,
            cancellationToken: cancellationToken);

        eventConverter
            .Received()
            .FromEvents(
                events,
                expected,
                Arg.Any<StreamWriteOptions>());
    }

    [Theory, AutoNSubstituteData]
    internal async ValueTask Should_Return_State_From_EventWriter(
        [Frozen, Substitute] IStreamBatchWriter eventWriter,
        StreamWriter sut,
        StreamId streamId,
        IReadOnlyList<object> events,
        StreamMetadata expected,
        CancellationToken cancellationToken)
    {
        eventWriter
            .WriteAsync(default, default)
            .ReturnsForAnyArgs(Task.FromResult<IStreamMetadata>(expected));

        var result = await sut.WriteAsync(
            streamId,
            events,
            StreamVersion.Any,
            options: null,
            cancellationToken: cancellationToken);

        result
            .Should()
            .BeEquivalentTo(expected);
    }
}