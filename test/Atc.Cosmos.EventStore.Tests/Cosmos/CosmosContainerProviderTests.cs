using Atc.Cosmos.EventStore.Cosmos;
using Atc.Test;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests.Cosmos
{
    public class CosmosContainerProviderTests
    {
        [Theory, AutoNSubstituteData]
        internal void Should_Provide_StreamContainer(
            [Frozen] ICosmosClientFactory cosmosFactory,
            [Substitute] CosmosClient cosmosClient,
            [Substitute] IOptions<EventStoreClientOptions> options,
            [Substitute] Container container)
        {
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

            sut.GetStreamContainer()
                .Should()
                .Be(container);

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

            sut.GetSubscriptionContainer()
                .Should()
                .Be(container);

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

            sut.GetIndexContainer()
                .Should()
                .Be(container);

            cosmosClient
                .Received(1)
                .GetContainer(
                    options.Value.EventStoreDatabaseId,
                    options.Value.IndexContainerId);
        }
    }
}