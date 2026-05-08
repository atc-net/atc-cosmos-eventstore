namespace Atc.Cosmos.EventStore.Streams.Tests;

public sealed class StreamBatchTests
{
    [Theory]
    [InlineAutoNSubstituteData]
    internal void Constructor_WithValidMetadataAndEvents_ReturnsInstance(
        StreamMetadata metadata,
        List<EventDocument> events)
    {
        // Act
        var sut = new StreamBatch(metadata, events);

        // Assert
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
        // Act
        var act = () => new StreamBatch(
            metadata,
            Enumerable.Repeat(events[0], 100).ToList());

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }
}