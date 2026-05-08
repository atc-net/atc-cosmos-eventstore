using static Atc.Cosmos.EventStore.Cqrs.Tests.Internal.ConsumeEventMetadataTestSpec;

namespace Atc.Cosmos.EventStore.Cqrs.Tests.Internal;

public sealed class ConsumeAnyEventAsyncMetadataTests :
    IClassFixture<ConsumeEventMetadataFixture<ConsumesAnyEventAsync>>
{
    private readonly ConsumeEventMetadataFixture<ConsumesAnyEventAsync> fixture;

    public ConsumeAnyEventAsyncMetadataTests(
        ConsumeEventMetadataFixture<ConsumesAnyEventAsync> fixture)
        => this.fixture = fixture;

    [Fact]
    public void IsNotConsumingEvents_Should_False_When_Implementing_IConsumeAnyEventAsync()
    {
        // Act
        var result = fixture.IsNotConsumingEvents();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Should_Consume_Any_Events()
    {
        // Act
        var result = fixture.CanConsumeEvent(fixture.ConsumableEven);

        // Assert
        result.Should().BeTrue();
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