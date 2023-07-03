using Atc.Cosmos.EventStore.Events;
using FluentAssertions;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests.Events;

public class EventDocumentExtensionsTests
{
    [Theory]
    [InlineData(10)]
    [InlineData(50)]
    public void ThrowIfLimitExceeded_ShouldNotThrow(int eventsCount)
    {
        var events = new List<object>();
        for (int i = 0; i < eventsCount; i++)
        {
            events.Add(new object());
        }

        events.ThrowIfEventLimitExceeded().Should().BeEquivalentTo(events);
    }

    [Theory]
    [InlineData(1000)]
    [InlineData(1500)]
    public void ThrowIfLimitExceeded_ShouldThrow(int eventsCount)
    {
        var events = new List<object>();
        for (int i = 0; i < eventsCount; i++)
        {
            events.Add(new object());
        }

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            events.ThrowIfEventLimitExceeded();
        });

        ex.Should().NotBeNull();
    }
}