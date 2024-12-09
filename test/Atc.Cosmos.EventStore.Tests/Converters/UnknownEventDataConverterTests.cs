using System.Text.Json;
using Atc.Cosmos.EventStore.Converters;
using Atc.Cosmos.EventStore.Tests.Fakes;
using Atc.Test;
using FluentAssertions;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests.Converters;

public class UnknownEventDataConverterTests
{
    private readonly JsonDocument doc = JsonDocument.Parse(JsonSerializer.Serialize(new EventOne("name", 42)));

    [Theory, AutoNSubstituteData]
    internal void Should_Return_Converted_Value_Id_NotNull(
        IEventMetadata metadata,
        string expected,
        UnknownEventDataConverter sut)
        => sut
            .Convert(
                metadata,
                doc.RootElement,
                new JsonSerializerOptions(),
                () => expected)
            .Should()
            .Be(expected);

    [Theory, AutoNSubstituteData]
    internal void Should_Return_UnknownEvent_When_Value_IsNot_Converted(
        IEventMetadata metadata,
        UnknownEventDataConverter sut)
        => sut
            .Convert(
                metadata,
                doc.RootElement,
                new JsonSerializerOptions(),
                () => null)
            .Should()
            .BeEquivalentTo(
                new UnknownEvent(doc.RootElement.GetRawText()));
}
