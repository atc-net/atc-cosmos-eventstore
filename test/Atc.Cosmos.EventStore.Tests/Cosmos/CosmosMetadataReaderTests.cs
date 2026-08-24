namespace Atc.Cosmos.EventStore.Tests.Cosmos;

public sealed class CosmosMetadataReaderTests
{
    private readonly Container container;
    private readonly StreamMetadata expectedMetadata;
    private readonly DateTimeOffset expectedTimestamp;
    private readonly string expectedETag;
    private readonly IEventStoreContainerProvider containerProvider;
    private readonly IDateTimeProvider dateTimeProvider;
    private readonly CosmosMetadataReader sut;
    private HttpStatusCode statusCode = HttpStatusCode.OK;

    public CosmosMetadataReaderTests()
    {
        expectedMetadata = new Fixture().Create<StreamMetadata>();
        expectedETag = new Fixture().Create<string>();

        var serializer = Substitute.For<CosmosEventSerializer>(
            Options.Create(new EventStoreClientOptions()),
            Substitute.For<IEventTypeProvider>());

        serializer
            .FromStream<StreamMetadata>(Arg.Any<Stream>())
            .Returns(expectedMetadata);

        container = Substitute.For<Container>();
        container
            .ReadItemStreamAsync(null, default, null, CancellationToken.None)
            .ReturnsForAnyArgs(_ => CreateResponse());

        expectedTimestamp = DateTimeOffset.UtcNow;
        dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider
            .GetDateTime()
            .Returns(expectedTimestamp);

        containerProvider = Substitute.For<IEventStoreContainerProvider>();
        containerProvider
            .GetStreamContainer()
            .Returns(container, returnThese: null);

        sut = new CosmosMetadataReader(containerProvider, dateTimeProvider, serializer);
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
            .ReadItemStreamAsync(
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
            .ReadItemStreamAsync(
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

    [Theory, AutoNSubstituteData]
    public Task Should_Propagate_CosmosException_When_StatusCode_IsNot_NotFound(
        StreamId streamId)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // Arrange
        statusCode = HttpStatusCode.TooManyRequests;

        // Act
        var act = () => sut.GetAsync(streamId, cancellationToken);

        // Assert
        return act
            .Should()
            .ThrowAsync<CosmosException>();
    }

    [Theory, AutoNSubstituteData]
    public async Task Should_Have_State_New_When_Document_IsNotFound(
        StreamId streamId)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // Arrange
        statusCode = HttpStatusCode.NotFound;

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

    [Theory, AutoNSubstituteData]
    public async Task Should_Have_StreamVersion_StartOfStream_When_Document_IsNotFound(
        StreamId streamId)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // Arrange
        statusCode = HttpStatusCode.NotFound;

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

    [Theory, AutoNSubstituteData]
    public async Task Should_Have_Correct_Id_When_Document_IsNotFound(
        StreamId streamId)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // Arrange
        statusCode = HttpStatusCode.NotFound;

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

    [Theory, AutoNSubstituteData]
    public async Task Should_Have_StreamId_As_PartitionKey_When_Document_IsNotFound(
        StreamId streamId)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // Arrange
        statusCode = HttpStatusCode.NotFound;

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

    [Theory, AutoNSubstituteData]
    public async Task Should_Have_Timestamp_When_Document_IsNotFound(
        StreamId streamId)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // Arrange
        statusCode = HttpStatusCode.NotFound;

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

    private ResponseMessage CreateResponse()
    {
        var response = new ResponseMessage(statusCode)
        {
            Content = new MemoryStream("{}"u8.ToArray()),
        };

        response.Headers.Add("etag", expectedETag);

        return response;
    }
}