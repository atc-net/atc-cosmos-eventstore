namespace Atc.Cosmos.EventStore.Tests.Cosmos;

public sealed class CosmosDeleterTests
{
    private readonly ResponseMessage responseMessage;
    private readonly Container container;
    private readonly IEventStoreContainerProvider containerProvider;
    private readonly CosmosDeleter sut;

    public CosmosDeleterTests()
    {
        responseMessage = Substitute.For<ResponseMessage>();
        responseMessage.IsSuccessStatusCode.Returns(true);

        container = Substitute.For<Container>();
        container
            .DeleteAllItemsByPartitionKeyStreamAsync(default, default, default)
            .ReturnsForAnyArgs(responseMessage);

        containerProvider = Substitute.For<IEventStoreContainerProvider>();
        containerProvider
            .GetStreamContainer()
            .Returns(container, returnThese: null);

        sut = new CosmosDeleter(containerProvider);
    }

    [Theory, AutoNSubstituteData]
    public async Task Should_Use_StreamId_As_PartitionKey(
        StreamId streamId,
        CancellationToken cancellationToken)
    {
        // Act
        await sut.DeleteAsync(
            streamId,
            cancellationToken);

        // Assert
        _ = container
            .Received()
            .DeleteAllItemsByPartitionKeyStreamAsync(
                new PartitionKey(streamId.Value),
                null,
                cancellationToken);
    }
}