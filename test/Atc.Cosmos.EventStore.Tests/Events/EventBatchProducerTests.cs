using Atc.Cosmos.EventStore.Events;
using Atc.Cosmos.EventStore.Streams;
using Atc.Test;
using AutoFixture;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests.Events;

public class EventBatchProducerTests
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

    public class TestEvent
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
    internal void Can_Convert_One_Event(
        EventBatchProducer sut)
    {
        var batch = sut.FromEvents(
            new[] { @event },
            metadata,
            options);

        batch
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
        var batch = sut.FromEvents(
            new[] { event1, event2, event3 },
            metadata,
            options);

        batch
            .Documents
            .Should()
            .HaveCount(3);
    }

    [Fact]
    public void Should_Have_NextVersion()
        => convertedEvent
            .Properties
            .Version
            .Should()
            .Be(metadata.Version.Value + 1);

    [Fact]
    public void Id_Should_Be_PropertyVersion()
        => convertedEvent
            .Id
            .Should()
            .Be($"{(long)convertedEvent.Properties.Version}");

    [Fact]
    public void PartitionKey_Should_Be_StreamId()
        => convertedEvent
            .PartitionKey
            .Should()
            .Be(convertedEvent.Properties.StreamId.Value);

    [Fact]
    public void Should_Set_StreamId()
        => convertedEvent
            .Properties
            .StreamId
            .Should()
            .Be(metadata.StreamId);

    [Fact]
    public void Should_Have_Event_Object_Set_As_Data()
        => convertedEvent
            .Data
            .Should()
            .Be(@event);

    [Fact]
    public void Should_Properties()
        => convertedEvent
            .Properties
            .Should()
            .NotBeNull();

    [Fact]
    public void Should_Have_Properties_CausationId_From_Options()
        => convertedEvent
            .Properties
            .CausationId
            .Should()
            .Be(options.CausationId);

    [Theory, AutoNSubstituteData]
    internal void Should_Allow_Null_CausationId(
        EventBatchProducer sut)
    {
        var batch = sut.FromEvents(
            new[] { @event },
            metadata,
            options: null);

        batch
            .Documents
            .First()
            .Properties
            .CausationId
            .Should()
            .BeNull();
    }

    [Fact]
    public void Should_Have_Properties_CorrelationId_From_Options()
        => convertedEvent
            .Properties
            .CorrelationId
            .Should()
            .Be(options.CorrelationId);

    [Theory, AutoNSubstituteData]
    internal void Should_Allow_Null_CorrelationId(
        EventBatchProducer sut)
    {
        var batch = sut.FromEvents(
            new[] { @event },
            metadata,
            options: null);

        batch
            .Documents
            .First()
            .Properties
            .CorrelationId
            .Should()
            .BeNull();
    }

    [Fact]
    public void Should_Set_Timestamp()
        => convertedEvent
            .Properties
            .Timestamp
            .Should()
            .Be(expectedTimestamp);

    [Fact]
    public void Should_Set_Name()
        => convertedEvent
            .Properties
            .Name
            .Should()
            .Be(expectedName);

    [Fact]
    public void Should_Have_Metadata_State_Active()
        => convertedMetadata
            .State
            .Should()
            .Be(StreamState.Active);
}