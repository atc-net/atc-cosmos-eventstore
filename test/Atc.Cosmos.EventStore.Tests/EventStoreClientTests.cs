using System.Collections.ObjectModel;
using Atc.Cosmos.EventStore.Cosmos;
using Atc.Cosmos.EventStore.Streams;
using Atc.Test;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests;

public class EventStoreClientTests
{
    [Theory, AutoNSubstituteData]
    internal async Task Should_DeleteSubscription(
        [Frozen] IStreamSubscriptionRemover remover,
        EventStoreClient sut,
        ConsumerGroup consumerGroup,
        CancellationToken cancellationToken)
    {
        await sut.DeleteSubscriptionAsync(
            consumerGroup,
            cancellationToken: cancellationToken);

        _ = remover
                .Received(1)
                .DeleteAsync(
                    consumerGroup,
                    cancellationToken);
    }

    [Theory, AutoNSubstituteData]
    internal Task Should_Throw_On_DeleteSubscription_When_ConsumerGroup_IsNull(
        EventStoreClient sut,
        CancellationToken cancellationToken)
        => FluentActions
            .Awaiting(() => sut.DeleteSubscriptionAsync(null, cancellationToken))
            .Should()
            .ThrowAsync<ArgumentNullException>();

    [Theory, AutoNSubstituteData]
    internal async Task Should_Return_Info_On_GetStreamInfo(
        [Frozen] IStreamInfoReader reader,
        EventStoreClient sut,
        StreamId streamId,
        IStreamMetadata expectedResult,
        CancellationToken cancellationToken)
    {
        reader
            .ReadAsync(default, default)
            .ReturnsForAnyArgs(expectedResult);

        var info = await sut.GetStreamInfoAsync(
            streamId,
            cancellationToken: cancellationToken);

        info
            .Should()
            .Be(expectedResult);
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_WriteToStream(
        [Frozen, Substitute] IStreamWriter writer,
        EventStoreClient sut,
        StreamId streamId,
        IReadOnlyList<object> events,
        StreamResponse expected,
        CancellationToken cancellationToken)
    {
        writer
            .WriteAsync(default, default, default, default, default)
            .ReturnsForAnyArgs(Task.FromResult<StreamResponse>(expected));

        var result = await sut.WriteToStreamAsync(
            streamId,
            events,
            StreamVersion.StartOfStream,
            cancellationToken: cancellationToken);

        result
            .Should()
            .BeEquivalentTo(expected);
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Throw_InvalidOperationException(
        EventStoreClient sut,
        StreamId streamId,
        IReadOnlyList<object> events,
        CancellationToken cancellationToken)
    {
        await sut
            .Invoking(
                c => c.WriteToStreamAsync(
                    streamId,
                    Enumerable.Repeat(events[0], CosmosConstants.BatchLimit).ToList(),
                    StreamVersion.StartOfStream,
                    cancellationToken: cancellationToken))
            .Should()
            .ThrowExactlyAsync<InvalidOperationException>();
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Throw_When_EventsList_Contains_NullObject(
        EventStoreClient sut,
        StreamId streamId,
        Collection<object> events,
        CancellationToken cancellationToken)
    {
        events.Add(null);

        await FluentActions
            .Awaiting(() => sut.WriteToStreamAsync(
                streamId,
                events,
                StreamVersion.StartOfStream,
                cancellationToken: cancellationToken))
            .Should()
            .ThrowAsync<ArgumentException>();
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_SetStreamCheckpoint(
        [Frozen] IStreamCheckpointWriter writer,
        EventStoreClient sut,
        string name,
        StreamId streamId,
        StreamVersion streamVersion,
        object state,
        CancellationToken cancellationToken)
    {
        await sut.SetStreamCheckpointAsync(
            name,
            streamId,
            streamVersion,
            state,
            cancellationToken);

        _ = writer
                .Received(1)
                .WriteAsync(
                    name,
                    streamId,
                    streamVersion,
                    state,
                    cancellationToken);
    }

    [Theory, AutoNSubstituteData]
    internal Task Should_Throw_On_SetStreamCheckpoint_When_Name_IsNull(
        EventStoreClient sut,
        StreamId streamId,
        StreamVersion streamVersion,
        CancellationToken cancellationToken)
        => FluentActions
            .Awaiting(() => sut.SetStreamCheckpointAsync(null, streamId, streamVersion, null, cancellationToken))
            .Should()
            .ThrowAsync<ArgumentNullException>();

    [Theory, AutoNSubstituteData]
    internal async Task Should_GetStreamCheckpoint_With_State(
        [Frozen] IStreamCheckpointReader reader,
        EventStoreClient sut,
        string name,
        StreamId streamId,
        Checkpoint<string> expectedCheckpoint,
        CancellationToken cancellationToken)
    {
        reader
            .ReadAsync<string>(default, default, default)
            .ReturnsForAnyArgs(expectedCheckpoint);

        var checkpoint = await sut.GetStreamCheckpointAsync<string>(
            name,
            streamId,
            cancellationToken);

        _ = reader
                .Received(1)
                .ReadAsync<string>(
                    name,
                    streamId,
                    cancellationToken);

        checkpoint
            .Should()
            .Be(expectedCheckpoint);
    }

    [Theory, AutoNSubstituteData]
    internal Task Should_Throw_On_GetStreamCheckpoint_With_State_When_Name_IsNull(
        EventStoreClient sut,
        StreamId streamId,
        CancellationToken cancellationToken)
        => FluentActions
            .Awaiting(() => sut.GetStreamCheckpointAsync<string>(null, streamId, cancellationToken))
            .Should()
            .ThrowAsync<ArgumentNullException>();

    [Theory, AutoNSubstituteData]
    internal async Task Should_GetStreamCheckpoint_Without_State(
        [Frozen] IStreamCheckpointReader reader,
        EventStoreClient sut,
        string name,
        StreamId streamId,
        Checkpoint<object> expectedCheckpoint,
        CancellationToken cancellationToken)
    {
        reader
            .ReadAsync<object>(default, default, default)
            .ReturnsForAnyArgs(expectedCheckpoint);

        var checkpoint = await sut.GetStreamCheckpointAsync(
            name,
            streamId,
            cancellationToken);

        checkpoint
            .Should()
            .Be(expectedCheckpoint);
    }

    [Theory, AutoNSubstituteData]
    internal Task Should_Throw_On_GetStreamCheckpoint_Without_State_When_Name_IsNull(
        EventStoreClient sut,
        StreamId streamId,
        CancellationToken cancellationToken)
        => FluentActions
            .Awaiting(() => sut.GetStreamCheckpointAsync(null, streamId, cancellationToken))
            .Should()
            .ThrowAsync<ArgumentNullException>();

    [Theory, AutoNSubstituteData]
    internal async Task Should_DeleteStream(
        [Frozen] IStreamDeleter deleter,
        EventStoreClient sut,
        StreamId streamId,
        CancellationToken cancellationToken)
    {
        await sut.DeleteStreamAsync(
            streamId,
            cancellationToken: cancellationToken);

        _ = deleter
            .Received(1)
            .DeleteAsync(
                streamId,
                cancellationToken);
    }
}