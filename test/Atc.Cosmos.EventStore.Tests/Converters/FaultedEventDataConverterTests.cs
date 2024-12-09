using System.Text.Json;
using Atc.Cosmos.EventStore.Converters;
using Atc.Cosmos.EventStore.Tests.Fakes;
using Atc.Test;
using FluentAssertions;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests.Converters;

public class FaultedEventDataConverterTests
{
    private readonly JsonDocument doc = JsonDocument.Parse(JsonSerializer.Serialize(new EventOne("name", 42)));

    [Theory, AutoNSubstituteData]
    internal void Should_Return_Converted_Value(
        IEventMetadata metadata,
        string expected,
        FaultedEventDataConverter sut)
        => sut
            .Convert(
                metadata,
                doc.RootElement,
                new JsonSerializerOptions(),
                () => expected)
            .Should()
            .Be(expected);

    [Theory, AutoNSubstituteData]
    internal void Should_Return_FaultedEvent_When_Exception_IsThrown(
        IEventMetadata metadata,
        KeyNotFoundException exception,
        FaultedEventDataConverter sut)
        => sut
            .Convert(
                metadata,
                doc.RootElement,
                new JsonSerializerOptions(),
                () => throw exception)
            .Should()
            .BeEquivalentTo(
                new FaultedEvent(
                    doc.RootElement.GetRawText(),
                    exception));
}
