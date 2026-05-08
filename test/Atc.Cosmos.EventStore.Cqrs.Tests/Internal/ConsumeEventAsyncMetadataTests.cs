using static Atc.Cosmos.EventStore.Cqrs.Tests.Internal.ConsumeEventMetadataTestSpec;

namespace Atc.Cosmos.EventStore.Cqrs.Tests.Internal;

public sealed class ConsumeEventAsyncMetadataTests :
    IClassFixture<ConsumeEventMetadataFixture<ConsumesOneEventAsync>>
{
    private readonly ConsumeEventMetadataFixture<ConsumesOneEventAsync> fixture;

    public ConsumeEventAsyncMetadataTests(
        ConsumeEventMetadataFixture<ConsumesOneEventAsync> fixture)
        => this.fixture = fixture;

    [Fact]
    public void IsNotConsumingEvents_Should_False_When_OneOrMore_IConsume_Interfaces_AreImplemented()
    {
        // Act
        var result = fixture.IsNotConsumingEvents();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanConsumeEvent_Should_Return_True_When_Event_Is_Consumed()
    {
        // Act
        var result = fixture.CanConsumeEvent(fixture.ConsumableEven);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanConsumeEvent_Should_Return_False_When_Event_IsNot_Consumed()
    {
        // Act
        var result = fixture.CanConsumeEvent(fixture.NotConsumableEven);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Should_Project_Consumed_Event()
    {
        // Act
        var result = await fixture.ConsumeEventAsync(fixture.ConsumableEven);

        // Assert
        result.Projection.EventConsumed.Should().Be(fixture.ConsumableEven.Data);
    }

    [Fact]
    public async Task Should_Project_Consumed_Metadata()
    {
        // Act
        var result = await fixture.ConsumeEventAsync(fixture.ConsumableEven);

        // Assert
        result.Projection.MetadataConsumed
            .Should()
            .BeEquivalentTo(
                new EventMetadata(
                    EventStreamId.FromStreamId(fixture.ConsumableEven.Metadata.StreamId),
                    fixture.ConsumableEven.Metadata.Timestamp,
                    (long)fixture.ConsumableEven.Metadata.Version,
                    CorrelationId: fixture.ConsumableEven.Metadata.CorrelationId,
                    CausationId: fixture.ConsumableEven.Metadata.CausationId));
    }
}