using Atc.Cosmos.EventStore.Cosmos;
using Atc.Cosmos.EventStore.Events;
using Atc.Cosmos.EventStore.Streams;
using Atc.Test;
using AutoFixture;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using NSubstitute;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests.Cosmos;

public class CosmosBatchWriterTests
{
    private readonly TransactionalBatchOperationResult<StreamMetadata> operationResponse;
    private readonly Container container;
    private readonly TransactionalBatch transactionBatch;
    private readonly StreamMetadata expectedMetadata;
    private readonly TransactionalBatchResponse expectedTransactionResponse;
    private readonly string expectedETag;
    private readonly IEventStoreContainerProvider containerProvider;
    private readonly IDateTimeProvider dateTimeProvider;
    private readonly DateTimeOffset expectedDateTime;
    private readonly CosmosBatchWriter sut;

    public CosmosBatchWriterTests()
    {
        expectedMetadata = new Fixture().Create<StreamMetadata>();
        expectedETag = new Fixture().Create<string>();
        operationResponse = Substitute.For<TransactionalBatchOperationResult<StreamMetadata>>();
        operationResponse
            .Resource
            .Returns(expectedMetadata);
        operationResponse
            .ETag
            .Returns(expectedETag);

        expectedTransactionResponse = Substitute.For<TransactionalBatchResponse>();
        expectedTransactionResponse
            .IsSuccessStatusCode
            .Returns(returnThis: true);
        expectedTransactionResponse
            .GetOperationResultAtIndex<StreamMetadata>(default)
            .ReturnsForAnyArgs(operationResponse);

        transactionBatch = Substitute.For<TransactionalBatch>();
        transactionBatch
            .UpsertItem<StreamMetadata>(default, default)
            .ReturnsForAnyArgs(transactionBatch);
        transactionBatch
            .CreateItem<EventDocument>(default, default)
            .ReturnsForAnyArgs(transactionBatch);
        transactionBatch
            .ExecuteAsync(default)
            .ReturnsForAnyArgs(Task.FromResult(expectedTransactionResponse));

        container = Substitute.For<Container>();
        container
            .CreateTransactionalBatch(default)
            .ReturnsForAnyArgs(transactionBatch);

        containerProvider = Substitute.For<IEventStoreContainerProvider>();
        containerProvider
            .GetStreamContainer()
            .Returns(container, returnThese: null);
        containerProvider
            .GetIndexContainer()
            .Returns(container, returnThese: null);

        expectedDateTime = DateTimeOffset.UtcNow;
        dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider
            .GetDateTime()
            .Returns(expectedDateTime);

        sut = new CosmosBatchWriter(containerProvider, dateTimeProvider);
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Create_TransactionBatch_With_StreamId_as_PartitionKey(
        StreamBatch streamBatch,
        CancellationToken cancellationToken)
    {
        await sut.WriteAsync(streamBatch, cancellationToken);

        _ = container
            .Received()
            .CreateTransactionalBatch(
                new PartitionKey(streamBatch.Metadata.StreamId.Value));
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Upsert_Metadata_With_ETag(
        StreamBatch streamBatch,
        CancellationToken cancellationToken)
    {
        await sut.WriteAsync(streamBatch, cancellationToken);

        _ = transactionBatch
            .Received()
            .UpsertItem<StreamMetadata>(
                streamBatch.Metadata,
                Arg.Is<TransactionalBatchItemRequestOptions>(opt => opt.IfMatchEtag == streamBatch.Metadata.ETag));
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Create_Documents(
        StreamBatch streamBatch,
        CancellationToken cancellationToken)
    {
        await sut.WriteAsync(streamBatch, cancellationToken);

        _ = transactionBatch
            .Received(streamBatch.Documents.Count)
            .CreateItem<EventDocument>(
                Arg.Any<EventDocument>(),
                Arg.Any<TransactionalBatchItemRequestOptions>());
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Throw_When_ExecutionFails(
        StreamBatch streamBatch,
        CancellationToken cancellationToken)
    {
        expectedTransactionResponse
            .IsSuccessStatusCode
            .Returns(returnThis: false);

        await FluentActions
            .Awaiting(() => sut.WriteAsync(streamBatch, cancellationToken))
            .Should()
            .ThrowAsync<StreamWriteConflictException>();
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Create_Index_When_Stream_IsEmpty(
        StreamBatch streamBatch,
        CancellationToken cancellationToken)
    {
        streamBatch.Metadata.ETag = null;

        await sut.WriteAsync(streamBatch, cancellationToken);

        _ = container
            .Received()
            .UpsertItemAsync<StreamIndex>(
                Arg.Is<StreamIndex>(arg => arg.PartitionKey == nameof(StreamIndex) && arg.StreamId == streamBatch.Metadata.StreamId),
                new PartitionKey(nameof(StreamIndex)),
                Arg.Is<ItemRequestOptions>(arg => arg.EnableContentResponseOnWrite == false),
                cancellationToken);
    }
}