namespace Atc.Cosmos.EventStore.Tests;

public sealed class StreamIdTests
{
    [Theory, AutoNSubstituteData]
    public void Should_Be_Constructed_With_Id(
        [Frozen] string id,
        StreamId sut)
    {
        // Act
        var result = sut.Value;

        // Assert
        result
            .Should()
            .Be(id);
    }

    [Theory, AutoNSubstituteData]
    [SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Needed by test")]
    [SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "Needed by test")]
    public void Should_Be_EqualTo(
        [Frozen] string id, // The same id will be injected into both left and right.
        StreamId left,
        StreamId right)
    {
        // Act
        var result = left == right;

        // Assert
        result
            .Should()
            .BeTrue();
    }

    [Theory, AutoNSubstituteData]
    public void Should_Be_EqualTo_Using_String_On_LeftSide(
        [Frozen] string id,
        StreamId right)
    {
        // Act
        var result = id == right;

        // Assert
        result
            .Should()
            .BeTrue();
    }

    [Theory, AutoNSubstituteData]
    public void Should_Be_EqualTo_Using_String_On_RightSide(
        [Frozen] string id,
        StreamId left)
    {
        // Act
        var result = left == id;

        // Assert
        result
            .Should()
            .BeTrue();
    }

    [Theory, AutoNSubstituteData]
    public void Should_Support_Explicit_String_Overload(
        [Frozen] string id,
        StreamId sut)
    {
        // Act
        var result = (string)sut;

        // Assert
        result
            .Should()
            .Be(id);
    }

    [Theory, AutoNSubstituteData]
    public void Should_Support_Getting_StreamId_As_String(
        [Frozen] string id,
        StreamId sut)
    {
        // Act
        var result = StreamId.FromStreamId(sut);

        // Assert
        result
            .Should()
            .Be(id);
    }

    [Theory, AutoNSubstituteData]
    public void Should_Support_Getting_StreamId_Using_String(
        [Frozen] string id,
        StreamId sut)
    {
        // Act
        var result = StreamId.ToStreamId(id);

        // Assert
        result
            .Should()
            .BeEquivalentTo(sut);
    }
}