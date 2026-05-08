namespace Atc.Cosmos.EventStore.Tests.Converters;

public sealed class UnknownEventDataConverterTests
{
    private readonly JsonDocument doc = JsonDocument.Parse(JsonSerializer.Serialize(new EventOne("name", 42)));

    [Theory, AutoNSubstituteData]
    internal void Should_Return_Converted_Value_Id_NotNull(
        IEventMetadata metadata,
        string expected,
        UnknownEventDataConverter sut)
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
    internal void Should_Return_UnknownEvent_When_Value_IsNot_Converted(
        IEventMetadata metadata,
        UnknownEventDataConverter sut)
    {
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
                new UnknownEvent(doc.RootElement.GetRawText()));
    }
}