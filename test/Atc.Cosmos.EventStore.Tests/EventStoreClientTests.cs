namespace Atc.Cosmos.EventStore.Tests;

public sealed class EventStoreClientTests
{
    [Theory, AutoNSubstituteData]
    internal async Task Should_DeleteSubscription(
        [Frozen] IStreamSubscriptionRemover remover,
        EventStoreClient sut,
        ConsumerGroup consumerGroup,
        CancellationToken cancellationToken)
    {
        // Act
        await sut.DeleteSubscriptionAsync(
            consumerGroup,
            cancellationToken: cancellationToken);

        // Assert
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
    {
        // Act
        var act = () => sut.DeleteSubscriptionAsync(null, cancellationToken);

        // Assert
        return act
            .Should()
            .ThrowAsync<ArgumentNullException>();
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Return_Info_On_GetStreamInfo(
        [Frozen] IStreamInfoReader reader,
        EventStoreClient sut,
        StreamId streamId,
        IStreamMetadata expectedResult,
        CancellationToken cancellationToken)
    {
        // Arrange
        reader
            .ReadAsync(default, default)
            .ReturnsForAnyArgs(expectedResult);

        // Act
        var info = await sut.GetStreamInfoAsync(
            streamId,
            cancellationToken: cancellationToken);

        // Assert
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
        // Arrange
        writer
            .WriteAsync(default, default, default, default, default)
            .ReturnsForAnyArgs(Task.FromResult<StreamResponse>(expected));

        // Act
        var result = await sut.WriteToStreamAsync(
            streamId,
            events,
            StreamVersion.StartOfStream,
            cancellationToken: cancellationToken);

        // Assert
        result
            .Should()
            .BeEquivalentTo(expected);
    }

    [Theory, AutoNSubstituteData]
    internal Task Should_Throw_InvalidOperationException(
        EventStoreClient sut,
        StreamId streamId,
        IReadOnlyList<object> events,
        CancellationToken cancellationToken)
    {
        // Act
        var act = () => sut.WriteToStreamAsync(
            streamId,
            Enumerable.Repeat(events[0], CosmosConstants.BatchLimit).ToList(),
            StreamVersion.StartOfStream,
            cancellationToken: cancellationToken);

        // Assert
        return act
            .Should()
            .ThrowExactlyAsync<InvalidOperationException>();
    }

    [Theory, AutoNSubstituteData]
    internal Task Should_Throw_When_EventsList_Contains_NullObject(
        EventStoreClient sut,
        StreamId streamId,
        Collection<object> events,
        CancellationToken cancellationToken)
    {
        // Arrange
        events.Add(null);

        // Act
        var act = () => sut.WriteToStreamAsync(
            streamId,
            events,
            StreamVersion.StartOfStream,
            cancellationToken: cancellationToken);

        // Assert
        return act
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
        // Act
        await sut.SetStreamCheckpointAsync(
            name,
            streamId,
            streamVersion,
            state,
            cancellationToken);

        // Assert
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
    {
        // Act
        var act = () => sut.SetStreamCheckpointAsync(null, streamId, streamVersion, null, cancellationToken);

        // Assert
        return act
            .Should()
            .ThrowAsync<ArgumentNullException>();
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_GetStreamCheckpoint_With_State(
        [Frozen] IStreamCheckpointReader reader,
        EventStoreClient sut,
        string name,
        StreamId streamId,
        Checkpoint<string> expectedCheckpoint,
        CancellationToken cancellationToken)
    {
        // Arrange
        reader
            .ReadAsync<string>(default, default, default)
            .ReturnsForAnyArgs(expectedCheckpoint);

        // Act
        var checkpoint = await sut.GetStreamCheckpointAsync<string>(
            name,
            streamId,
            cancellationToken);

        // Assert
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
    {
        // Act
        var act = () => sut.GetStreamCheckpointAsync<string>(null, streamId, cancellationToken);

        // Assert
        return act
            .Should()
            .ThrowAsync<ArgumentNullException>();
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_GetStreamCheckpoint_Without_State(
        [Frozen] IStreamCheckpointReader reader,
        EventStoreClient sut,
        string name,
        StreamId streamId,
        Checkpoint<object> expectedCheckpoint,
        CancellationToken cancellationToken)
    {
        // Arrange
        reader
            .ReadAsync<object>(default, default, default)
            .ReturnsForAnyArgs(expectedCheckpoint);

        // Act
        var checkpoint = await sut.GetStreamCheckpointAsync(
            name,
            streamId,
            cancellationToken);

        // Assert
        checkpoint
            .Should()
            .Be(expectedCheckpoint);
    }

    [Theory, AutoNSubstituteData]
    internal Task Should_Throw_On_GetStreamCheckpoint_Without_State_When_Name_IsNull(
        EventStoreClient sut,
        StreamId streamId,
        CancellationToken cancellationToken)
    {
        // Act
        var act = () => sut.GetStreamCheckpointAsync(null, streamId, cancellationToken);

        // Assert
        return act
            .Should()
            .ThrowAsync<ArgumentNullException>();
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_DeleteStream(
        [Frozen] IStreamDeleter deleter,
        EventStoreClient sut,
        StreamId streamId,
        CancellationToken cancellationToken)
    {
        // Act
        await sut.DeleteStreamAsync(
            streamId,
            cancellationToken: cancellationToken);

        // Assert
        _ = deleter
            .Received(1)
            .DeleteAsync(
                streamId,
                cancellationToken);
    }
}