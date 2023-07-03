using Atc.Cosmos.EventStore.Events;
using Atc.Test;
using FluentAssertions;
using Xunit;

namespace Atc.Cosmos.EventStore.Cqrs.Commands.Tests;

public class CommandContextTests
{
    [Theory, AutoNSubstituteData]
    internal void EventsShouldReturnEmptyList_WhenNoEventsAdded(
        CommandContext sut)
    {
        var actual = sut.Events;
        actual.Should().BeEmpty();
    }

    [Theory, AutoNSubstituteData]
    internal void AddEventShouldAddEventToAppliedEventsList(
        CommandContext sut,
        object eventData)
    {
        sut.AddEvent(eventData);
        sut.Events.Should().Contain(eventData);
    }

    [Theory, AutoNSubstituteData]
    internal void AddEventShouldThrowException_WhenAppliedEventsCountExceedsLimit(
        CommandContext sut,
        object eventData)
    {
        for (int i = 0; i < CommandContext.EventLimit; i++)
        {
            sut.AddEvent(eventData);
        }

        sut.Invoking(c => c.AddEvent(new object()))
            .Should()
            .Throw<InvalidOperationException>();
    }
}
