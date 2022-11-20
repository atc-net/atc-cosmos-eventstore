using FluentAssertions;
using Xunit;
using static Atc.Cosmos.EventStore.Cqrs.Tests.Internal.ConsumeEventMetadataTestSpec;

namespace Atc.Cosmos.EventStore.Cqrs.Tests.Internal;

public class ConsumeEventMetadataTests :
    IClassFixture<ConsumeEventMetadataFixture<ConsumesOneEvent>>
{
    private readonly ConsumeEventMetadataFixture<ConsumesOneEvent> fixture;

    public ConsumeEventMetadataTests(
        ConsumeEventMetadataFixture<ConsumesOneEvent> fixture)
        => this.fixture = fixture;

    [Fact]
    public void IsNotConsumingEvents_Should_False_When_OneOrMore_IConsume_Interfaces_AreImplemented()
        => fixture
            .IsNotConsumingEvents()
            .Should()
            .BeFalse();

    [Fact]
    public void CanConsumeEvent_Should_Return_True_When_Event_Is_Consumed()
        => fixture
            .CanConsumeEvent(fixture.ConsumableEven)
            .Should()
            .BeTrue();

    [Fact]
    public void CanConsumeEvent_Should_Return_False_When_Event_IsNot_Consumed()
        => fixture
            .CanConsumeEvent(fixture.NotConsumableEven)
            .Should()
            .BeFalse();

    [Fact]
    public async Task Should_Project_Consumed_Event()
        => (await fixture
            .ConsumeEventAsync(fixture.ConsumableEven))
            .Projection
            .EventConsumed
            .Should()
            .Be(fixture.ConsumableEven.Data);

    [Fact]
    public async Task Should_Project_Consumed_Metadata()
        => (await fixture
            .ConsumeEventAsync(fixture.ConsumableEven))
            .Projection
            .MetadataConsumed
                .Should()
                .BeEquivalentTo(
                    new EventMetadata(
                        fixture.ConsumableEven.Metadata.EventId,
                        EventStreamId.FromStreamId(fixture.ConsumableEven.Metadata.StreamId),
                        fixture.ConsumableEven.Metadata.Timestamp,
                        (long)fixture.ConsumableEven.Metadata.Version,
                        CorrelationId: fixture.ConsumableEven.Metadata.CorrelationId,
                        CausationId: fixture.ConsumableEven.Metadata.CausationId));
}
