namespace Atc.Cosmos.EventStore.Tests.Events;

public sealed class EventBatchProducerTests
{
    private readonly IDateTimeProvider dateTimeProvider;
    private readonly IEventNameProvider nameProvider;
    private readonly EventBatchProducer sut;
    private readonly StreamMetadata metadata;
    private readonly StreamWriteOptions options;
    private readonly DateTimeOffset expectedTimestamp;
    private readonly string expectedName;
    private readonly TestEvent @event;
    private readonly EventDocument convertedEvent;
    private readonly StreamMetadata convertedMetadata;

    public sealed class TestEvent
    {
        public string Id { get; set; }
    }

    public EventBatchProducerTests()
    {
        dateTimeProvider = Substitute.For<IDateTimeProvider>();
        nameProvider = Substitute.For<IEventNameProvider>();
        expectedName = "event-name";
        expectedTimestamp = DateTimeOffset.Now;
        metadata = new StreamMetadata(
            "id",
            "pk",
            "streamId",
            1,
            StreamState.Active,
            expectedTimestamp.AddDays(-1));
        options = new StreamWriteOptions
        {
            CausationId = "A",
            CorrelationId = "B",
        };
        @event = new Fixture().Create<TestEvent>();

        dateTimeProvider
            .GetDateTime()
            .Returns(expectedTimestamp);
        nameProvider
            .GetName(default)
            .ReturnsForAnyArgs(expectedName);

        sut = new EventBatchProducer(dateTimeProvider, nameProvider);
        var batch = sut.FromEvents(
            new[] { @event },
            metadata,
            options);
        convertedEvent = batch.Documents.First();
        convertedMetadata = batch.Metadata;
    }

    [Theory, AutoNSubstituteData]
    internal void Can_Convert_One_Event(EventBatchProducer sut)
    {
        // Act
        var result = sut.FromEvents(
            new[] { @event },
            metadata,
            options);

        // Assert
        result
            .Documents
            .Should()
            .HaveCount(1);
    }

    [Theory, AutoNSubstituteData]
    internal void Can_Convert_Multiple_Events(
        TestEvent event1,
        TestEvent event2,
        TestEvent event3,
        EventBatchProducer sut)
    {
        // Act
        var result = sut.FromEvents(
            new[] { event1, event2, event3 },
            metadata,
            options);

        // Assert
        result
            .Documents
            .Should()
            .HaveCount(3);
    }

    [Fact]
    public void Should_Have_NextVersion()
    {
        // Act
        var result = convertedEvent.Properties.Version;

        // Assert
        result
            .Should()
            .Be(metadata.Version.Value + 1);
    }

    [Fact]
    public void Id_Should_Be_PropertyVersion()
    {
        // Act
        var result = convertedEvent.Id;

        // Assert
        result
            .Should()
            .Be($"{(long)convertedEvent.Properties.Version}");
    }

    [Fact]
    public void PartitionKey_Should_Be_StreamId()
    {
        // Act
        var result = convertedEvent.PartitionKey;

        // Assert
        result
            .Should()
            .Be(convertedEvent.Properties.StreamId.Value);
    }

    [Fact]
    public void Should_Set_StreamId()
    {
        // Act
        var result = convertedEvent.Properties.StreamId;

        // Assert
        result
            .Should()
            .Be(metadata.StreamId);
    }

    [Fact]
    public void Should_Have_Event_Object_Set_As_Data()
    {
        // Act
        var result = convertedEvent.Data;

        // Assert
        result
            .Should()
            .Be(@event);
    }

    [Fact]
    public void Should_Properties()
    {
        // Act
        var result = convertedEvent.Properties;

        // Assert
        result
            .Should()
            .NotBeNull();
    }

    [Fact]
    public void Should_Have_Properties_CausationId_From_Options()
    {
        // Act
        var result = convertedEvent.Properties.CausationId;

        // Assert
        result
            .Should()
            .Be(options.CausationId);
    }

    [Theory, AutoNSubstituteData]
    internal void Should_Allow_Null_CausationId(EventBatchProducer sut)
    {
        // Act
        var result = sut.FromEvents(
            new[] { @event },
            metadata,
            options: null);

        // Assert
        result
            .Documents
            .First()
            .Properties
            .CausationId
            .Should()
            .BeNull();
    }

    [Fact]
    public void Should_Have_Properties_CorrelationId_From_Options()
    {
        // Act
        var result = convertedEvent.Properties.CorrelationId;

        // Assert
        result
            .Should()
            .Be(options.CorrelationId);
    }

    [Theory, AutoNSubstituteData]
    internal void Should_Allow_Null_CorrelationId(EventBatchProducer sut)
    {
        // Act
        var result = sut.FromEvents(
            new[] { @event },
            metadata,
            options: null);

        // Assert
        result
            .Documents
            .First()
            .Properties
            .CorrelationId
            .Should()
            .BeNull();
    }

    [Fact]
    public void Should_Set_Timestamp()
    {
        // Act
        var result = convertedEvent.Properties.Timestamp;

        // Assert
        result
            .Should()
            .Be(expectedTimestamp);
    }

    [Fact]
    public void Should_Set_Name()
    {
        // Act
        var result = convertedEvent.Properties.Name;

        // Assert
        result
            .Should()
            .Be(expectedName);
    }

    [Fact]
    public void Should_Have_Metadata_State_Active()
    {
        // Act
        var result = convertedMetadata.State;

        // Assert
        result
            .Should()
            .Be(StreamState.Active);
    }

    [Theory, AutoNSubstituteData]
    internal void Throws_If_Limit_Exceeded(
        IReadOnlyCollection<TestEvent> events,
        EventBatchProducer sut)
    {
        // Act
        var act = sut.Invoking(
            x => x
                .FromEvents(
                    Enumerable.Repeat(events, 100).ToList(),
                    metadata,
                    options));

        // Assert
        act
            .Should()
            .ThrowExactly<InvalidOperationException>();
    }
}