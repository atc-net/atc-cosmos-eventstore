namespace Atc.Cosmos.EventStore.Tests.Events;

public sealed class EventDocumentExtensionsTests
{
    [Theory]
    [InlineData(10)]
    [InlineData(50)]
    public void ThrowIfLimitExceeded_ShouldNotThrow(int eventsCount)
    {
        // Arrange
        var events = new List<object>();
        for (int i = 0; i < eventsCount; i++)
        {
            events.Add(new object());
        }

        // Act
        var result = events.ThrowIfEventLimitExceeded();

        // Assert
        result.Should().BeEquivalentTo(events);
    }

    [Theory]
    [InlineData(1000)]
    [InlineData(1500)]
    public void ThrowIfLimitExceeded_ShouldThrow(int eventsCount)
    {
        // Arrange
        var events = new List<object>();
        for (int i = 0; i < eventsCount; i++)
        {
            events.Add(new object());
        }

        // Act
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            events.ThrowIfEventLimitExceeded();
        });

        // Assert
        ex.Should().NotBeNull();
    }
}