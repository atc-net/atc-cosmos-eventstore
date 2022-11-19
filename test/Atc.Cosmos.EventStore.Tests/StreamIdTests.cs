using System.Diagnostics.CodeAnalysis;
using Atc.Test;
using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests;

public class StreamIdTests
{
    [Theory, AutoNSubstituteData]
    public void Should_Be_Constructed_With_Id(
        [Frozen] string id,
        StreamId sut)
        => sut
            .Value
            .Should()
            .Be(id);

    [Theory, AutoNSubstituteData]
    [SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Needed by test")]
    [SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "Needed by test")]
    public void Should_Be_EqualTo(
        [Frozen] string id, // The same id will be injected into both left and right.
        StreamId left,
        StreamId right)
        => (left == right)
            .Should()
            .BeTrue();

    [Theory, AutoNSubstituteData]
    public void Should_Be_EqualTo_Using_String_On_LeftSide(
        [Frozen] string id,
        StreamId right)
        => (id == right)
            .Should()
            .BeTrue();

    [Theory, AutoNSubstituteData]
    public void Should_Be_EqualTo_Using_String_On_RightSide(
        [Frozen] string id,
        StreamId left)
        => (left == id)
            .Should()
            .BeTrue();

    [Theory, AutoNSubstituteData]
    public void Should_Support_Explicit_String_Overload(
        [Frozen] string id,
        StreamId sut)
        => ((string)sut)
            .Should()
            .Be(id);

    [Theory, AutoNSubstituteData]
    public void Should_Support_Getting_StreamId_As_String(
        [Frozen] string id,
        StreamId sut)
        => StreamId
            .FromStreamId(sut)
            .Should()
            .Be(id);

    [Theory, AutoNSubstituteData]
    public void Should_Support_Getting_StreamId_Using_String(
        [Frozen] string id,
        StreamId sut)
        => StreamId
            .ToStreamId(id)
            .Should()
            .BeEquivalentTo(sut);
}