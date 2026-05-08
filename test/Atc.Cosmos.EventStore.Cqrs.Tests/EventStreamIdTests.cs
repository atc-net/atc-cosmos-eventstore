namespace Atc.Cosmos.EventStore.Cqrs.Commands.Tests;

public sealed class EventStreamIdTests
{
    [Fact]
    public void Ctor_must_throw_when_no_arguments_are_provided()
    {
        // Act
        var act = () => new EventStreamId();

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Value_property_must_return_joined_parts()
    {
        // Arrange
        var id = new EventStreamId("foo", "bar");

        // Act
        var result = id.Value;

        // Assert
        result.Should().Be("foo.bar");
    }

    [Fact]
    public void Parts_property_must_return_parts()
    {
        // Arrange
        var id = new EventStreamId("foo", "bar");

        // Act
        var result = id.Parts;

        // Assert
        result.Should().BeEquivalentTo("foo", "bar");
    }

    [Fact]
    public void EventStreamId_can_be_cloned_using_ctor()
    {
        // Arrange
        var id = new EventStreamId("foo", "bar");
        var clone = new EventStreamId(id);

        // Act
        var result = id.Value;

        // Assert
        result.Should().Be(clone.Value);
    }

    [Fact]
    public void EventStreamId_can_be_created_from_StreamId()
    {
        // Arrange
        var streamId = new StreamId("foo.bar");

        // Act
        EventStreamId eventStreamId = streamId;
        EventStreamId eventStreamId2 = EventStreamId.FromStreamId(streamId);

        // Assert
        eventStreamId.Value.Should().Be("foo.bar");
        eventStreamId2.Value.Should().Be("foo.bar");
    }
}