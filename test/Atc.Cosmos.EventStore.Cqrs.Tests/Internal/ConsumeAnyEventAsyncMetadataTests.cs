using FluentAssertions;
using Xunit;
using static Atc.Cosmos.EventStore.Cqrs.Tests.Internal.ConsumeEventMetadataTestSpec;

namespace Atc.Cosmos.EventStore.Cqrs.Tests.Internal;

public class ConsumeAnyEventAsyncMetadataTests :
    IClassFixture<ConsumeEventMetadataFixture<ConsumesAnyEventAsync>>
{
    private readonly ConsumeEventMetadataFixture<ConsumesAnyEventAsync> fixture;

    public ConsumeAnyEventAsyncMetadataTests(
        ConsumeEventMetadataFixture<ConsumesAnyEventAsync> fixture)
        => this.fixture = fixture;

    [Fact]
    public void IsNotConsumingEvents_Should_False_When_Implementing_IConsumeAnyEventAsync()
        => fixture
            .IsNotConsumingEvents()
            .Should()
            .BeFalse();

    [Fact]
    public void Should_Consume_Any_Events()
        => fixture
            .CanConsumeEvent(fixture.ConsumableEven)
            .Should()
            .BeTrue();

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
