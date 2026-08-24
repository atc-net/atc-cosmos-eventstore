namespace Atc.Cosmos.EventStore.Tests.Cosmos;

public sealed class CosmosCheckpointReaderTests
{
    private readonly Container container;
    private readonly CheckpointDocument<string> expectedCheckpoint;
    private readonly IEventStoreContainerProvider containerProvider;
    private readonly CosmosCheckpointReader sut;
    private HttpStatusCode statusCode = HttpStatusCode.OK;

    public CosmosCheckpointReaderTests()
    {
        expectedCheckpoint = new Fixture().Create<CheckpointDocument<string>>();

        var serializer = Substitute.For<CosmosEventSerializer>(
            Options.Create(new EventStoreClientOptions()),
            Substitute.For<IEventTypeProvider>());

        serializer
            .FromStream<CheckpointDocument<string>>(Arg.Any<Stream>())
            .Returns(expectedCheckpoint);

        container = Substitute.For<Container>();

        container
            .ReadItemStreamAsync(id: null, partitionKey: default, requestOptions: null, CancellationToken.None)
            .ReturnsForAnyArgs(_ => new ResponseMessage(statusCode)
            {
                Content = new MemoryStream("{}"u8.ToArray()),
            });

        containerProvider = Substitute.For<IEventStoreContainerProvider>();

        containerProvider
            .GetIndexContainer()
            .Returns(container, returnThese: null);

        sut = new CosmosCheckpointReader(containerProvider, serializer);
    }

    [Theory, AutoNSubstituteData]
    public async Task Should_Read_From_Index_Container(
        string name,
        StreamId streamId)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        await sut
            .ReadAsync<string>(name, streamId, cancellationToken);

        // Assert
        containerProvider
            .Received(1)
            .GetIndexContainer();
    }

    [Theory, AutoNSubstituteData]
    public async Task Should_Use_Name_As_DocumentId(
        string name,
        StreamId streamId)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        await sut
            .ReadAsync<string>(name, streamId, cancellationToken);

        // Assert
        _ = container
            .Received(1)
            .ReadItemStreamAsync(
                name,
                Arg.Any<PartitionKey>(),
                Arg.Any<ItemRequestOptions>(),
                cancellationToken);
    }

    [Theory, AutoNSubstituteData]
    public async Task Should_Use_StreamId_As_PartitionKey(
        string name,
        StreamId streamId)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        await sut
            .ReadAsync<string>(name, streamId, cancellationToken);

        // Assert
        _ = container
            .Received(1)
            .ReadItemStreamAsync(
                Arg.Any<string>(),
                Arg.Is<PartitionKey>(pk => pk == new PartitionKey(streamId.Value)),
                Arg.Any<ItemRequestOptions>(),
                cancellationToken);
    }

    [Theory, AutoNSubstituteData]
    public async Task Should_Return_Null_When_Document_IsNotFound(
        string name,
        StreamId streamId)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // Arrange
        statusCode = HttpStatusCode.NotFound;

        // Act
        var checkpoint = await sut.ReadAsync<string>(
            name,
            streamId,
            cancellationToken);

        // Assert
        checkpoint
            .Should()
            .BeNull();
    }

    [Theory, AutoNSubstituteData]
    public Task Should_Propagate_CosmosException_When_StatusCode_IsNot_NotFound(
        string name,
        StreamId streamId)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // Arrange
        statusCode = HttpStatusCode.TooManyRequests;

        // Act
        var act = () => sut.ReadAsync<string>(name, streamId, cancellationToken);

        // Assert
        return act
            .Should()
            .ThrowAsync<CosmosException>();
    }

    [Theory, AutoNSubstituteData]
    public async Task Should_Return_Checkpoint(
        string name,
        StreamId streamId)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        var checkpoint = await sut.ReadAsync<string>(
            name,
            streamId,
            cancellationToken);

        // Assert
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