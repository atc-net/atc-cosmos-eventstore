using System.Net;
using Atc.Cosmos.EventStore.Cosmos;
using Atc.Test;
using AutoFixture;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests.Cosmos;

public class CosmosCheckpointReaderTests
{
    private readonly ItemResponse<CheckpointDocument<string>> itemResponse;
    private readonly Container container;
    private readonly CheckpointDocument<string> expectedCheckpoint;
    private readonly IEventStoreContainerProvider containerProvider;
    private readonly CosmosCheckpointReader sut;

    public CosmosCheckpointReaderTests()
    {
        expectedCheckpoint = new Fixture().Create<CheckpointDocument<string>>();
        itemResponse = Substitute.For<ItemResponse<CheckpointDocument<string>>>();
        itemResponse
            .Resource
            .Returns(expectedCheckpoint);

        container = Substitute.For<Container>();
        container
            .ReadItemAsync<CheckpointDocument<string>>(default, default, default, default)
            .ReturnsForAnyArgs(itemResponse);

        containerProvider = Substitute.For<IEventStoreContainerProvider>();
        containerProvider
            .GetIndexContainer()
            .Returns(container, returnThese: null);

        sut = new CosmosCheckpointReader(containerProvider);
    }

    [Theory, AutoNSubstituteData]
    public async Task Should_Read_From_Index_Container(
        string name,
        StreamId streamId,
        CancellationToken cancellationToken)
    {
        await sut
            .ReadAsync<string>(name, streamId, cancellationToken);

        containerProvider
            .Received(1)
            .GetIndexContainer();
    }

    [Theory, AutoNSubstituteData]
    public async Task Should_Use_Name_As_DocumentId(
        string name,
        StreamId streamId,
        CancellationToken cancellationToken)
    {
        await sut
            .ReadAsync<string>(name, streamId, cancellationToken);

        _ = container
            .Received(1)
            .ReadItemAsync<CheckpointDocument<string>>(
                name,
                Arg.Any<PartitionKey>(),
                Arg.Any<ItemRequestOptions>(),
                cancellationToken);
    }

    [Theory, AutoNSubstituteData]
    public async Task Should_Use_StreamId_As_PartitionKey(
        string name,
        StreamId streamId,
        CancellationToken cancellationToken)
    {
        await sut
            .ReadAsync<string>(name, streamId, cancellationToken);

        _ = container
            .Received(1)
            .ReadItemAsync<CheckpointDocument<string>>(
                Arg.Any<string>(),
                Arg.Is<PartitionKey>(pk => pk == new PartitionKey(streamId.Value)),
                Arg.Any<ItemRequestOptions>(),
                cancellationToken);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "NS5003:Synchronous exception thrown from async method.", Justification = "Reviewed")]
    [Theory, AutoNSubstituteData]
    public async Task Should_Return_Null_When_Document_IsNotFound(
        string name,
        StreamId streamId,
        CancellationToken cancellationToken)
    {
        container
            .ReadItemAsync<CheckpointDocument<string>>(default, default, default, default)
            .ThrowsForAnyArgs(new CosmosException("error", HttpStatusCode.NotFound, 0, "a", 1));

        var checkpoint = await sut.ReadAsync<string>(
            name,
            streamId,
            cancellationToken);

        checkpoint
            .Should()
            .BeNull();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "NS5003:Synchronous exception thrown from async method.", Justification = "Reviewed")]
    [Theory, AutoNSubstituteData]
    public async Task Should_Propergate_CosmosException_When_StatusCode_IsNot_NotFound(
        string name,
        StreamId streamId,
        CancellationToken cancellationToken)
    {
        container
            .ReadItemAsync<CheckpointDocument<string>>(default, default, default, default)
            .ThrowsForAnyArgs(new CosmosException("error", HttpStatusCode.TooManyRequests, 0, "a", 1));

        await FluentActions
            .Awaiting(() => sut.ReadAsync<string>(name, streamId, cancellationToken))
            .Should()
            .ThrowAsync<CosmosException>();
    }

    [Theory, AutoNSubstituteData]
    public async Task Should_Return_Checkpoint(
        string name,
        StreamId streamId,
        CancellationToken cancellationToken)
    {
        var checkpoint = await sut.ReadAsync<string>(
            name,
            streamId,
            cancellationToken);

        checkpoint
            .Should()
            .BeEquivalentTo(
                new Checkpoint<string>(
                    expectedCheckpoint.Name,
                    expectedCheckpoint.StreamId,
                    expectedCheckpoint.StreamVersion,
                    expectedCheckpoint.Timestamp,
                    expectedCheckpoint.State));
    }
}