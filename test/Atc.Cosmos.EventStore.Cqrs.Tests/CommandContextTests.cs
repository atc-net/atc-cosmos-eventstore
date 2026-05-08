namespace Atc.Cosmos.EventStore.Cqrs.Commands.Tests;

public sealed class CommandContextTests
{
    [Theory, AutoNSubstituteData]
    internal void EventsShouldReturnEmptyList_WhenNoEventsAdded(
        CommandContext sut)
    {
        // Act
        var result = sut.Events;

        // Assert
        result.Should().BeEmpty();
    }

    [Theory, AutoNSubstituteData]
    internal void AddEventShouldAddEventToAppliedEventsList(
        CommandContext sut,
        object eventData)
    {
        // Act
        sut.AddEvent(eventData);

        // Assert
        sut.Events.Should().Contain(eventData);
    }

    [Theory, AutoNSubstituteData]
    internal void AddEventShouldThrowException_WhenAppliedEventsCountExceedsLimit(
        CommandContext sut,
        object eventData)
    {
        // Arrange
        for (int i = 0; i < CommandContext.EventLimit; i++)
        {
            sut.AddEvent(eventData);
        }

        // Act
        var act = () => sut.AddEvent(new object());

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }
}