using System.Text.Json;
using Atc.Cosmos.EventStore.Converters;
using Atc.Cosmos.EventStore.Tests.Fakes;
using Atc.Test;
using FluentAssertions;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests.Converters;

public class EventDataConverterPipelineBuilderTests
{
    private readonly JsonDocument doc = JsonDocument.Parse(JsonSerializer.Serialize(new EventOne("name", 42)));

    [Theory, AutoNSubstituteData]
    internal void Should_Call_All_Converters_InReverseOrder(
        IEventMetadata metadata,
        FakeEventDataConverter[] converters,
        FakeEventDataConverter converter,
        EventDataConverterPipelineBuilder sut)
        => sut
            .AddConverter(converter)
            .AddConverters(converters)
            .Build()
            .Convert(metadata, doc.RootElement, new JsonSerializerOptions())
            .Should()
            .Be(string.Join(string.Empty, new[] { converter }.Concat(converters).Select(c => c.Val)));
}
