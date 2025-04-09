using FluentAssertions;
using Xunit;

namespace Atc.Cosmos.EventStore.Cqrs.Commands.Tests;

public class EventStreamIdTests
{
    [Fact]
    public void Ctor_must_throw_when_no_arguments_are_provided()
    {
        Assert.Throws<ArgumentException>(() => new EventStreamId());
    }

    [Fact]
    public void Value_property_must_return_joined_parts()
    {
        var id = new EventStreamId("foo", "bar");
        id.Value.Should().Be("foo.bar");
    }

    [Fact]
    public void Parts_property_must_return_parts()
    {
        var id = new EventStreamId("foo", "bar");
        id.Parts.Should().BeEquivalentTo("foo", "bar");
    }

    [Fact]
    public void EventStreamId_can_be_cloned_using_ctor()
    {
        var id = new EventStreamId("foo", "bar");
        var clone = new EventStreamId(id);
        id.Value.Should().Be(clone.Value);
    }

    [Fact]
    public void EventStreamId_can_be_created_from_StreamId()
    {
        var streamId = new StreamId("foo.bar");
        EventStreamId eventStreamId = streamId;
        EventStreamId eventStreamId2 = EventStreamId.FromStreamId(streamId);
        eventStreamId.Value.Should().Be("foo.bar");
        eventStreamId2.Value.Should().Be("foo.bar");
    }
}