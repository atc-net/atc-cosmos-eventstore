using System.Text.Json;
using Atc.Cosmos.EventStore.Converters;
using Atc.Cosmos.EventStore.Events;
using Atc.Cosmos.EventStore.Tests.Fakes;
using Atc.Test;
using AutoFixture.Xunit2;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests.Converters;

public class NamedEventConverterTests
{
    private readonly JsonDocument doc = JsonDocument.Parse(JsonSerializer.Serialize(new EventOne("name", 42)));

    [Theory, AutoNSubstituteData]
    internal void Should_Return_Value_FromNext_When_TypeName_IsNotFound(
        [Frozen] IEventTypeProvider typeProvider,
        IEventMetadata metadata,
        string expected,
        NamedEventConverter sut)
    {
        typeProvider
            .GetEventType(default)
            .ReturnsForAnyArgs((Type)null);

        sut
            .Convert(
                metadata,
                doc.RootElement,
                new JsonSerializerOptions(),
                () => expected)
            .Should()
            .Be(expected);
    }

    [Theory, AutoNSubstituteData]
    internal void Should_Return_UnknownEvent_When_Value_IsNot_Converted(
        [Frozen] IEventTypeProvider typeProvider,
        IEventMetadata metadata,
        NamedEventConverter sut)
    {
        typeProvider
            .GetEventType(metadata.Name)
            .ReturnsForAnyArgs(typeof(EventOne));

        sut
            .Convert(
                metadata,
                doc.RootElement,
                new JsonSerializerOptions(),
                () => null)
            .Should()
            .BeEquivalentTo(
                new EventOne("name", 42));
    }
}
