namespace Atc.Cosmos.EventStore.Tests.Converters;

public sealed class FaultedEventDataConverterTests
{
    private readonly JsonDocument doc = JsonDocument.Parse(JsonSerializer.Serialize(new EventOne("name", 42)));

    [Theory, AutoNSubstituteData]
    internal void Should_Return_Converted_Value(
        IEventMetadata metadata,
        string expected,
        FaultedEventDataConverter sut)
    {
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
    internal void Should_Return_FaultedEvent_When_Exception_IsThrown(
        IEventMetadata metadata,
        KeyNotFoundException exception,
        FaultedEventDataConverter sut)
    {
        // Act
        var result = sut
            .Convert(
                metadata,
                doc.RootElement,
                new JsonSerializerOptions(),
                () => throw exception);

        // Assert
        result
            .Should()
            .BeEquivalentTo(
                new FaultedEvent(
                    doc.RootElement.GetRawText(),
                    exception));
    }
}