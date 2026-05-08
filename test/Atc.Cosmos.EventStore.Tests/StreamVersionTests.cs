namespace Atc.Cosmos.EventStore.Tests;

public sealed class StreamVersionTests
{
    [Fact]
    public void StartOfStream_Should_Have_Zero_Version_Value()
    {
        // Act
        var result = StreamVersion.StartOfStream.Value;

        // Assert
        result
            .Should()
            .Be(0);
    }

    [Fact]
    public void EndOfStream_Should_Have_MaxValue_Version_Value()
    {
        // Act
        var result = StreamVersion.EndOfStreamValue;

        // Assert
        result
            .Should()
            .Be(long.MaxValue);
    }

    [Theory, AutoNSubstituteData]
    public void Should_Be_Constructed_With_Version(
        [Frozen] long version,
        StreamVersion sut)
    {
        // Act
        var result = sut.Value;

        // Assert
        result
            .Should()
            .Be(version);
    }

    [Theory, AutoNSubstituteData]
    [SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Needed by test")]
    [SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "Needed by test")]
    public void Should_Be_Equal_When_InnerVersion_Has_The_Same_Value(
        [Frozen] long version,
        StreamVersion left,
        StreamVersion right)
    {
        // Act
        var result = left == right;

        // Assert
        result
            .Should()
            .BeTrue();
    }

    [Theory, AutoNSubstituteData]
    public void Should_Be_Equal_When_Comparing_Long_Value_With_StreamVersion(
        [Frozen] long version,
        StreamVersion sut)
    {
        // Act
        var result = sut == version;

        // Assert
        result
            .Should()
            .BeTrue();
    }

    [Theory, AutoNSubstituteData]
    public void Should_Construct_With_ToStreamVersion(
        [Frozen] long version,
        StreamVersion sut)
    {
        // Act
        var result = sut == StreamVersion.ToStreamVersion(version);

        // Assert
        result
            .Should()
            .BeTrue();
    }

    [Theory, AutoNSubstituteData]
    public void Can_Get_Version_VLong_Value_Explicit(
        [Frozen] long version,
        StreamVersion sut)
    {
        // Act
        var result = (long)sut;

        // Assert
        result
            .Should()
            .Be(version);
    }

    [Theory, AutoNSubstituteData]
    public void Can_Get_Version_As_Long_FromStreamVersion(
        [Frozen] long version,
        StreamVersion sut)
    {
        // Act
        var result = StreamVersion.FromStreamVersion(sut);

        // Assert
        result
            .Should()
            .Be(version);
    }

    [Theory, AutoNSubstituteData]
    public void Should_NotBe_EqualTo(
        [Frozen] long version,
        StreamVersion sut)
    {
        // Act
        var result = sut != StreamVersion.ToStreamVersion(version + 1);

        // Assert
        result
            .Should()
            .BeTrue();
    }

    [Theory, AutoNSubstituteData]
    public void Should_NotBe_EqualTo_With_Long(
        [Frozen] long version,
        StreamVersion sut)
    {
        // Act
        var result = sut != version + 1;

        // Assert
        result
            .Should()
            .BeTrue();
    }

    [Theory, AutoNSubstituteData]
    public void Should_Be_GreaterThan(
        [Frozen] long version,
        StreamVersion sut)
    {
        // Act
        var result = sut < version + 1;

        // Assert
        result
            .Should()
            .BeTrue();
    }

    [Theory, AutoNSubstituteData]
    public void Should_Be_LessThan(
        [Frozen] long version,
        StreamVersion sut)
    {
        // Act
        var result = sut > version - 1;

        // Assert
        result
            .Should()
            .BeTrue();
    }

    [Theory, AutoNSubstituteData]
    public void Should_Be_GreaterThan_Or_EqualTo(
        [Frozen] long version,
        StreamVersion sut)
    {
        // Act
        var result = sut <= version + 1;

        // Assert
        result
            .Should()
            .BeTrue();
    }

    [Theory, AutoNSubstituteData]
    public void Should_Be_LessThan_Or_EqualTo(
        [Frozen] long version,
        StreamVersion sut)
    {
        // Act
        var result = sut >= version - 1;

        // Assert
        result
            .Should()
            .BeTrue();
    }

    [Theory, AutoNSubstituteData]
    public void Should_Be_EqualTo_Using_CompareTo(
        [Frozen] long version,
        StreamVersion sut)
    {
        // Act
        var result = sut.CompareTo(version);

        // Assert
        result
            .Should()
            .Be(0);
    }

    [Theory, AutoNSubstituteData]
    public void Should_Be_GreaterThan_Using_CompareTo(
        [Frozen] long version,
        StreamVersion sut)
    {
        // Act
        var result = sut.CompareTo(version + 1);

        // Assert
        result
            .Should()
            .Be(-1);
    }

    [Theory, AutoNSubstituteData]
    public void Should_Be_LessThan_Using_CompareTo(
        [Frozen] long version,
        StreamVersion sut)
    {
        // Act
        var result = sut.CompareTo(version - 1);

        // Assert
        result
            .Should()
            .Be(1);
    }

    [Theory, AutoNSubstituteData]
    public void Should_Able_To_Sort_By_StreamVersion(StreamVersion[] sut)
    {
        // Act
        var result = sut.Order();

        // Assert
        result
            .Should()
            .ContainInOrder(
                sut
                    .OrderBy(s => s.Value)
                    .ToArray());
    }
}