namespace Atc.Cosmos.EventStore.Tests.Converters;

public sealed class NamedEventConverterTests
{
    private readonly JsonDocument doc = JsonDocument.Parse(JsonSerializer.Serialize(new EventOne("name", 42)));

    [Theory, AutoNSubstituteData]
    internal void Should_Return_Value_FromNext_When_TypeName_IsNotFound(
        [Frozen] IEventTypeProvider typeProvider,
        IEventMetadata metadata,
        string expected,
        NamedEventConverter sut)
    {
        // Arrange
        typeProvider
            .GetEventType(default)
            .ReturnsForAnyArgs((Type)null);

        // Act
        var result = sut
            .Convert(
                metadata,
                doc.RootElement,
                new JsonSerializerOptions(),
                () => expected);

        // Assert
        result
            .Should()
            .Be(expected);
    }

    [Theory, AutoNSubstituteData]
    internal void Should_Return_UnknownEvent_When_Value_IsNot_Converted(
        [Frozen] IEventTypeProvider typeProvider,
        IEventMetadata metadata,
        NamedEventConverter sut)
    {
        // Arrange
        typeProvider
            .GetEventType(metadata.Name)
            .ReturnsForAnyArgs(typeof(EventOne));

        // Act
        var result = sut
            .Convert(
                metadata,
                doc.RootElement,
                new JsonSerializerOptions(),
                () => null);

        // Assert
        result
            .Should()
            .BeEquivalentTo(
                new EventOne("name", 42));
    }
}