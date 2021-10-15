using System;
using System.Collections.Generic;
using System.Linq;
using Atc.Cosmos.EventStore.Events;
using Atc.Cosmos.EventStore.Tests.Fakes;
using Atc.Test;
using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests.Events
{
    public class EventCatalogTests
    {
        [Theory, AutoNSubstituteData]
        internal void Should_Resolve_Type_From_Name(
            [Frozen] IReadOnlyDictionary<EventName, Type> mappings,
            EventCatalog sut)
            => sut.GetEventType(mappings.Keys.First())
                .Should()
                .Be(mappings[mappings.Keys.First()]);

        [Theory, AutoNSubstituteData]
        internal void ShouldThrow_When_Name_IsNotFound(
            EventCatalog sut)
            => FluentActions
                .Invoking(() => sut.GetEventType("non-existing-name"))
                .Should()
                .Throw<EventNotRegisteredException>();

        [Theory, AutoNSubstituteData]
        public void Should_Resolve_Name_From_Type(
            EventOne evt1,
            string evt1Name,
            string evt2Name)
            => new EventCatalog(new Dictionary<EventName, Type>
                {
                    { evt1Name, typeof(EventOne) },
                    { evt2Name, typeof(EventTwo) },
                })
                .GetName(evt1)
                .Should()
                .Be(evt1Name);

        [Theory, AutoNSubstituteData]
        internal void ShouldThrow_When_Objects_Type_IsNotFound(
            EventOne evt,
            EventCatalog sut)
            => FluentActions
                .Invoking(() => sut.GetName(evt))
                .Should()
                .Throw<EventNotRegisteredException>();
    }
}