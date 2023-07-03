using Atc.Cosmos.EventStore.Events;
using Atc.Test;
using FluentAssertions;
using Xunit;

namespace Atc.Cosmos.EventStore.Streams.Tests;

public class StreamBatchTests
{
    [Theory]
    [InlineAutoNSubstituteData]
    internal void Constructor_WithValidMetadataAndEvents_ReturnsInstance(
        StreamMetadata metadata,
        List<EventDocument> events)
    {
        var sut = new StreamBatch(metadata, events);
        sut.Should().NotBeNull();
        sut.Metadata.Should().BeEquivalentTo(metadata);
        sut.Documents.Should().BeEquivalentTo(events);
    }

    [Theory]
    [InlineAutoNSubstituteData]
    internal void Constructor_WithEvents_ThrowsException_WhenEventLimitIsExceeded(
        StreamMetadata metadata,
        List<EventDocument> events)
    {
        var action = () => new StreamBatch(
            metadata,
            Enumerable.Repeat(events[0], 100).ToList());
        action.Should().Throw<InvalidOperationException>();
    }
}