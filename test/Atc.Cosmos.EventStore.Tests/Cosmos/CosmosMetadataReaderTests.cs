namespace Atc.Cosmos.EventStore.Tests.Cosmos;

public sealed class CosmosMetadataReaderTests
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
        StreamId streamId)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        await sut.GetAsync(streamId, cancellationToken);

        // Assert
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
        StreamId streamId)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        await sut.GetAsync(streamId, cancellationToken);

        // Assert
        _ = container
            .Received()
            .ReadItemAsync<StreamMetadata>(
                StreamMetadata.StreamMetadataId,
                Arg.Any<PartitionKey>(),
                Arg.Any<ItemRequestOptions>(),
                cancellationToken);
    }

    [Theory, AutoNSubstituteData]
    public async Task Should_Have_ETag_From_Response(StreamId streamId)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        var metadata = await sut
            .GetAsync(
                streamId,
                cancellationToken);

        // Assert
        metadata
            .ETag
            .Should()
            .Be(expectedETag);
    }

    [Theory, AutoNSubstituteData]
    public async Task Should_Return_Response(StreamId streamId)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        var metadata = await sut
            .GetAsync(
                streamId,
                cancellationToken);

        // Assert
        metadata
            .Should()
            .BeEquivalentTo(expectedMetadata);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "NS5003:Synchronous exception thrown from async method.", Justification = "Reviewed")]
    [Theory, AutoNSubstituteData]
    public async Task Should_Have_State_New_When_Document_IsNotFound(
        StreamId streamId)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // Arrange
        container
            .ReadItemAsync<StreamMetadata>(default, default, default, cancellationToken)
            .ThrowsForAnyArgs(new CosmosException("error", HttpStatusCode.NotFound, 0, "a", 1));

        // Act
        var metadata = await sut
            .GetAsync(
                streamId,
                cancellationToken);

        // Assert
        metadata
            .State
            .Should()
            .Be(StreamState.New);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "NS5003:Synchronous exception thrown from async method.", Justification = "Reviewed")]
    [Theory, AutoNSubstituteData]
    public async Task Should_Have_StreamVersion_StartOfStream_When_Document_IsNotFound(
        StreamId streamId)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // Arrange
        container
            .ReadItemAsync<StreamMetadata>(default, default, default, cancellationToken)
            .ThrowsForAnyArgs(new CosmosException("error", HttpStatusCode.NotFound, 0, "a", 1));

        // Act
        var metadata = await sut
            .GetAsync(
                streamId,
                cancellationToken);

        // Assert
        metadata
            .Version
            .Should()
            .Be(StreamVersion.StartOfStream);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "NS5003:Synchronous exception thrown from async method.", Justification = "Reviewed")]
    [Theory, AutoNSubstituteData]
    public async Task Should_Have_Correct_Id_When_Document_IsNotFound(
        StreamId streamId)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // Arrange
        container
            .ReadItemAsync<StreamMetadata>(default, default, default, cancellationToken)
            .ThrowsForAnyArgs(new CosmosException("error", HttpStatusCode.NotFound, 0, "a", 1));

        // Act
        var metadata = await sut
            .GetAsync(
                streamId,
                cancellationToken);

        // Assert
        ((StreamMetadata)metadata)
            .Id
            .Should()
            .Be(StreamMetadata.StreamMetadataId);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "NS5003:Synchronous exception thrown from async method.", Justification = "Reviewed")]
    [Theory, AutoNSubstituteData]
    public async Task Should_Have_StreamId_As_PartitionKey_When_Document_IsNotFound(
        StreamId streamId)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // Arrange
        container
            .ReadItemAsync<StreamMetadata>(default, default, default, cancellationToken)
            .ThrowsForAnyArgs(new CosmosException("error", HttpStatusCode.NotFound, 0, "a", 1));

        // Act
        var metadata = await sut
            .GetAsync(
                streamId,
                cancellationToken);

        // Assert
        ((StreamMetadata)metadata)
            .PartitionKey
            .Should()
            .Be(streamId.Value);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "NS5003:Synchronous exception thrown from async method.", Justification = "Reviewed")]
    [Theory, AutoNSubstituteData]
    public async Task Should_Have_Timestamp_When_Document_IsNotFound(
        StreamId streamId)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // Arrange
        container
            .ReadItemAsync<StreamMetadata>(default, default, default, cancellationToken)
            .ThrowsForAnyArgs(new CosmosException("error", HttpStatusCode.NotFound, 0, "a", 1));

        // Act
        var metadata = await sut
            .GetAsync(
                streamId,
                cancellationToken);

        // Assert
        metadata
            .Timestamp
            .Should()
            .Be(expectedTimestamp);
    }
}