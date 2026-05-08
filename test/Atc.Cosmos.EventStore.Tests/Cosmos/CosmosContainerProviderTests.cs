namespace Atc.Cosmos.EventStore.Tests.Cosmos;

public sealed class CosmosContainerProviderTests
{
    [Theory, AutoNSubstituteData]
    internal void Should_Provide_StreamContainer(
        [Frozen] ICosmosClientFactory cosmosFactory,
        [Substitute] CosmosClient cosmosClient,
        [Substitute] IOptions<EventStoreClientOptions> options,
        [Substitute] Container container)
    {
        // Arrange
        options
            .Value
            .Returns(new EventStoreClientOptions());
        cosmosClient
            .GetContainer(default, default)
            .ReturnsForAnyArgs(container);
        cosmosFactory
            .GetClient()
            .Returns(cosmosClient);
        var sut = new CosmosContainerProvider(cosmosFactory, options);

        // Act
        var result = sut.GetStreamContainer();

        // Assert
        result.Should().Be(container);
        cosmosClient
            .Received(1)
            .GetContainer(
                options.Value.EventStoreDatabaseId,
                options.Value.EventStoreContainerId);
    }

    [Theory, AutoNSubstituteData]
    internal void Should_Provide_SubscriptionContainer(
        [Frozen] ICosmosClientFactory cosmosFactory,
        [Substitute] CosmosClient cosmosClient,
        [Substitute] IOptions<EventStoreClientOptions> options,
        [Substitute] Container container)
    {
        // Arrange
        options
            .Value
            .Returns(new EventStoreClientOptions());
        cosmosClient
            .GetContainer(default, default)
            .ReturnsForAnyArgs(container);
        cosmosFactory
            .GetClient()
            .Returns(cosmosClient);
        var sut = new CosmosContainerProvider(cosmosFactory, options);

        // Act
        var result = sut.GetSubscriptionContainer();

        // Assert
        result.Should().Be(container);
        cosmosClient
            .Received(1)
            .GetContainer(
                options.Value.EventStoreDatabaseId,
                options.Value.SubscriptionContainerId);
    }

    [Theory, AutoNSubstituteData]
    internal void Should_Provide_IndexContainer(
        [Frozen] ICosmosClientFactory cosmosFactory,
        [Substitute] CosmosClient cosmosClient,
        [Substitute] IOptions<EventStoreClientOptions> options,
        [Substitute] Container container)
    {
        // Arrange
        options
            .Value
            .Returns(new EventStoreClientOptions());
        cosmosClient
            .GetContainer(default, default)
            .ReturnsForAnyArgs(container);
        cosmosFactory
            .GetClient()
            .Returns(cosmosClient);
        var sut = new CosmosContainerProvider(cosmosFactory, options);

        // Act
        var result = sut.GetIndexContainer();

        // Assert
        result.Should().Be(container);
        cosmosClient
            .Received(1)
            .GetContainer(
                options.Value.EventStoreDatabaseId,
                options.Value.IndexContainerId);
    }
}