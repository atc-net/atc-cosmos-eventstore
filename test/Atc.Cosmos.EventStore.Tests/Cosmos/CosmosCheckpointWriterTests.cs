namespace Atc.Cosmos.EventStore.Tests.Cosmos;

public sealed class CosmosCheckpointWriterTests
{
    private readonly ItemResponse<CheckpointDocument<string>> itemResponse;
    private readonly Container container;
    private readonly CheckpointDocument<string> expectedCheckpoint;
    private readonly DateTimeOffset expectedTimestamp;
    private readonly IDateTimeProvider dateTimeProvider;
    private readonly IEventStoreContainerProvider containerProvider;
    private readonly CosmosCheckpointWriter sut;

    public CosmosCheckpointWriterTests()
    {
        expectedCheckpoint = new Fixture().Create<CheckpointDocument<string>>();
        itemResponse = Substitute.For<ItemResponse<CheckpointDocument<string>>>();
        itemResponse
            .Resource
            .Returns(expectedCheckpoint);

        container = Substitute.For<Container>();
        container
            .UpsertItemAsync<CheckpointDocument<string>>(default, default, default, default)
            .ReturnsForAnyArgs(itemResponse);

        containerProvider = Substitute.For<IEventStoreContainerProvider>();
        containerProvider
            .GetIndexContainer()
            .Returns(container, returnThese: null);

        expectedTimestamp = DateTimeOffset.UtcNow;
        dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider
            .GetDateTime()
            .Returns(expectedTimestamp);

        sut = new CosmosCheckpointWriter(
            containerProvider,
            dateTimeProvider);
    }

    [Theory, AutoNSubstituteData]
    public async Task Should_Use_StreamId_As_PartitionKey(
        string name,
        StreamId streamId,
        StreamVersion streamVersion,
        string state,
        CancellationToken cancellationToken)
    {
        // Act
        await sut.WriteAsync(
            name,
            streamId,
            streamVersion,
            state,
            cancellationToken);

        // Assert
        _ = container
            .Received()
            .UpsertItemAsync<CheckpointDocument>(
                Arg.Any<CheckpointDocument>(),
                new PartitionKey(streamId.Value),
                Arg.Any<ItemRequestOptions>(),
                cancellationToken);
    }

    [Theory, AutoNSubstituteData]
    public async Task Should_Write_Checkpoint(
        string name,
        StreamId streamId,
        StreamVersion streamVersion,
        string state,
        CancellationToken cancellationToken)
    {
        // Arrange
        var expectedDocument = new CheckpointDocument(
            name,
            streamId,
            streamVersion,
            dateTimeProvider.GetDateTime(),
            state);

        // Act
        await sut.WriteAsync(
            name,
            streamId,
            streamVersion,
            state,
            cancellationToken);

        // Assert
        _ = container
            .Received()
            .UpsertItemAsync<CheckpointDocument>(
                Arg.Is<CheckpointDocument>(doc => doc == expectedDocument),
                Arg.Any<PartitionKey>(),
                Arg.Any<ItemRequestOptions>(),
                cancellationToken);
    }

    [Theory, AutoNSubstituteData]
    public async Task Should_Have_ContentResponse_Disabled(
        string name,
        StreamId streamId,
        StreamVersion streamVersion,
        string state,
        CancellationToken cancellationToken)
    {
        // Act
        await sut.WriteAsync(
            name,
            streamId,
            streamVersion,
            state,
            cancellationToken);

        // Assert
        _ = container
            .Received()
            .UpsertItemAsync<CheckpointDocument>(
                Arg.Any<CheckpointDocument>(),
                Arg.Any<PartitionKey>(),
                Arg.Is<ItemRequestOptions>(options => options.EnableContentResponseOnWrite == false),
                cancellationToken);
    }
}