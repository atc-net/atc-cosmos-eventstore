namespace Atc.Cosmos.EventStore.Tests.Events;

public sealed class EventCatalogTests
{
    [Theory, AutoNSubstituteData]
    internal void Should_Resolve_Type_From_Name(
        [Frozen] IReadOnlyDictionary<EventName, Type> mappings,
        EventCatalog sut)
    {
        // Act
        var result = sut.GetEventType(mappings.Keys.First());

        // Assert
        result
            .Should()
            .Be(mappings[mappings.Keys.First()]);
    }

    [Theory, AutoNSubstituteData]
    internal void ShouldReturn_Null_When_Name_IsNotFound(EventCatalog sut)
    {
        // Act
        var result = sut.GetEventType("non-existing-name");

        // Assert
        result
            .Should()
            .BeNull();
    }

    [Theory, AutoNSubstituteData]
    public void Should_Resolve_Name_From_Type(
        EventOne evt1,
        string evt1Name,
        string evt2Name)
    {
        // Arrange
        var sut = new EventCatalog(new Dictionary<EventName, Type>
        {
            { evt1Name, typeof(EventOne) },
            { evt2Name, typeof(EventTwo) },
        });

        // Act
        var result = sut.GetName(evt1);

        // Assert
        result
            .Should()
            .Be(evt1Name);
    }

    [Theory, AutoNSubstituteData]
    internal void ShouldThrow_When_Objects_Type_IsNotFound(
        EventOne evt,
        EventCatalog sut)
    {
        // Act
        var act = FluentActions
            .Invoking(() => sut.GetName(evt));

        // Assert
        act
            .Should()
            .Throw<EventNotRegisteredException>();
    }
}