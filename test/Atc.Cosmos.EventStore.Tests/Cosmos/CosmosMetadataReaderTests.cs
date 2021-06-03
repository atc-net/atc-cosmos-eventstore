using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Atc.Cosmos.EventStore.Cosmos;
using Atc.Cosmos.EventStore.Events;
using Atc.Cosmos.EventStore.Streams;
using Atc.Test;
using AutoFixture;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests.Cosmos
{
    public class CosmosMetadataReaderTests
    {
        private readonly ItemResponse<StreamMetadata> itemResponse;
        private readonly Container container;
        private readonly StreamMetadata expectedMetadata;
        private readonly DateTimeOffset expectedTimestamp;
        private readonly string expectedETag;
        private readonly IEventStoreContainerProvider containerProvider;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly CosmosMetadataReader sut;

        public CosmosMetadataReaderTests()
        {
            expectedMetadata = new Fixture().Create<StreamMetadata>();
            expectedETag = new Fixture().Create<string>();
            itemResponse = Substitute.For<ItemResponse<StreamMetadata>>();
            itemResponse
                .Resource
                .Returns(expectedMetadata);
            itemResponse
                .ETag
                .Returns(expectedETag);

            container = Substitute.For<Container>();
            container
                .ReadItemAsync<StreamMetadata>(default, default, default, default)
                .ReturnsForAnyArgs(itemResponse);

            expectedTimestamp = DateTimeOffset.UtcNow;
            dateTimeProvider = Substitute.For<IDateTimeProvider>();
            dateTimeProvider
                .GetDateTime()
                .Returns(expectedTimestamp);

            containerProvider = Substitute.For<IEventStoreContainerProvider>();
            containerProvider
                .GetStreamContainer()
                .Returns(container, returnThese: null);
            sut = new CosmosMetadataReader(containerProvider, dateTimeProvider);
        }

        [Theory, AutoNSubstituteData]
        public async Task Should_Use_StreamId_As_PartitionKey_When_ReadItem(
            StreamId streamId,
            CancellationToken cancellationToken)
        {
            await sut.GetAsync(streamId, cancellationToken);

            _ = container
                .Received()
                .ReadItemAsync<StreamMetadata>(
                    Arg.Any<string>(),
                    new PartitionKey(streamId.Value),
                    Arg.Any<ItemRequestOptions>(),
                    cancellationToken);
        }

        [Theory, AutoNSubstituteData]
        public async Task Should_Use_Fixed_Value_As_Id_When_ReadItem(
            StreamId streamId,
            CancellationToken cancellationToken)
        {
            await sut.GetAsync(streamId, cancellationToken);

            _ = container
                .Received()
                .ReadItemAsync<StreamMetadata>(
                    StreamMetadata.StreamMetadataId,
                    Arg.Any<PartitionKey>(),
                    Arg.Any<ItemRequestOptions>(),
                    cancellationToken);
        }

        [Theory, AutoNSubstituteData]
        public async Task Should_Have_ETag_From_Response(
            StreamId streamId,
            CancellationToken cancellationToken)
        {
            var metadata = await sut
                .GetAsync(
                    streamId,
                    cancellationToken);

            metadata
                .ETag
                .Should()
                .Be(expectedETag);
        }

        [Theory, AutoNSubstituteData]
        public async Task Should_Return_Response(
            StreamId streamId,
            CancellationToken cancellationToken)
        {
            var metadata = await sut
                .GetAsync(
                    streamId,
                    cancellationToken);

            metadata
                .Should()
                .BeEquivalentTo(expectedMetadata);
        }

        [Theory, AutoNSubstituteData]
        public async Task Should_Have_State_New_When_Document_IsNotFound(
            StreamId streamId,
            CancellationToken cancellationToken)
        {
            container
                .ReadItemAsync<StreamMetadata>(default, default, default, default)
                .ThrowsForAnyArgs(new CosmosException("error", HttpStatusCode.NotFound, 0, "a", 1));

            var metadata = await sut
                .GetAsync(
                    streamId,
                    cancellationToken);

            metadata
                .State
                .Should()
                .Be(StreamState.New);
        }

        [Theory, AutoNSubstituteData]
        public async Task Should_Have_StreamVersion_StartOfStream_When_Document_IsNotFound(
            StreamId streamId,
            CancellationToken cancellationToken)
        {
            container
                .ReadItemAsync<StreamMetadata>(default, default, default, default)
                .ThrowsForAnyArgs(new CosmosException("error", HttpStatusCode.NotFound, 0, "a", 1));

            var metadata = await sut
                .GetAsync(
                    streamId,
                    cancellationToken);

            metadata
                .Version
                .Should()
                .Be(StreamVersion.StartOfStream);
        }

        [Theory, AutoNSubstituteData]
        public async Task Should_Have_Correct_Id_When_Document_IsNotFound(
            StreamId streamId,
            CancellationToken cancellationToken)
        {
            container
                .ReadItemAsync<StreamMetadata>(default, default, default, default)
                .ThrowsForAnyArgs(new CosmosException("error", HttpStatusCode.NotFound, 0, "a", 1));

            var metadata = await sut
                .GetAsync(
                    streamId,
                    cancellationToken);

            ((StreamMetadata)metadata)
                .Id
                .Should()
                .Be(StreamMetadata.StreamMetadataId);
        }

        [Theory, AutoNSubstituteData]
        public async Task Should_Have_StreamId_As_PartitionKey_When_Document_IsNotFound(
            StreamId streamId,
            CancellationToken cancellationToken)
        {
            container
                .ReadItemAsync<StreamMetadata>(default, default, default, default)
                .ThrowsForAnyArgs(new CosmosException("error", HttpStatusCode.NotFound, 0, "a", 1));

            var metadata = await sut
                .GetAsync(
                    streamId,
                    cancellationToken);

            ((StreamMetadata)metadata)
                .PartitionKey
                .Should()
                .Be(streamId.Value);
        }

        [Theory, AutoNSubstituteData]
        public async Task Should_Have_Timestamp_When_Document_IsNotFound(
            StreamId streamId,
            CancellationToken cancellationToken)
        {
            container
                .ReadItemAsync<StreamMetadata>(default, default, default, default)
                .ThrowsForAnyArgs(new CosmosException("error", HttpStatusCode.NotFound, 0, "a", 1));

            var metadata = await sut
                .GetAsync(
                    streamId,
                    cancellationToken);

            metadata
                .Timestamp
                .Should()
                .Be(expectedTimestamp);
        }
    }
}