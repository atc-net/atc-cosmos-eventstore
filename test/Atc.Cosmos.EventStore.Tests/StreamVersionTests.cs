using System.Diagnostics.CodeAnalysis;
using Atc.Test;
using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests
{
    public class StreamVersionTests
    {
        [Fact]
        public void StartOfStream_Should_Have_Zero_Version_Value()
            => StreamVersion
                .StartOfStream
                .Value
                .Should()
                .Be(0);

        [Fact]
        public void EndOfStream_Should_Have_MaxValue_Version_Value()
            => StreamVersion
                .EndOfStreamValue
                .Should()
                .Be(long.MaxValue);

        [Theory, AutoNSubstituteData]
        public void Should_Be_Constructed_With_Version(
            [Frozen] long version,
            StreamVersion sut)
            => sut
                .Value
                .Should()
                .Be(version);

        [Theory, AutoNSubstituteData]
        [SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Needed by test")]
        [SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "Needed by test")]
        public void Should_Be_Equal_When_InnerVersion_Has_The_Same_Value(
            [Frozen] long version,
            StreamVersion left,
            StreamVersion right)
            => (left == right)
                .Should()
                .BeTrue();

        [Theory, AutoNSubstituteData]
        public void Should_Be_Equal_When_Comparing_Long_Value_With_StreamVersion(
            [Frozen] long version,
            StreamVersion sut)
            => (sut == version)
                .Should()
                .BeTrue();

        [Theory, AutoNSubstituteData]
        public void Should_Construct_With_ToStreamVersion(
            [Frozen] long version,
            StreamVersion sut)
            => (sut == StreamVersion.ToStreamVersion(version))
                .Should()
                .BeTrue();

        [Theory, AutoNSubstituteData]
        public void Can_Get_Version_VLong_Value_Explicit(
            [Frozen] long version,
            StreamVersion sut)
            => ((long)sut)
                .Should()
                .Be(version);

        [Theory, AutoNSubstituteData]
        public void Can_Get_Version_As_Long_FromStreamVersion(
            [Frozen] long version,
            StreamVersion sut)
            => StreamVersion
                .FromStreamVersion(sut)
                .Should()
                .Be(version);

        [Theory, AutoNSubstituteData]
        public void Should_NotBe_EqualTo(
            [Frozen] long version,
            StreamVersion sut)
            => (sut != StreamVersion.ToStreamVersion(version + 1))
                .Should()
                .BeTrue();

        [Theory, AutoNSubstituteData]
        public void Should_Be_EqualTo(
            [Frozen] long version,
            StreamVersion sut)
            => (sut == StreamVersion.ToStreamVersion(version))
                .Should()
                .BeTrue();

        [Theory, AutoNSubstituteData]
        public void Should_NotBe_EqualTo_With_Long(
            [Frozen] long version,
            StreamVersion sut)
            => (sut != version + 1)
                .Should()
                .BeTrue();

        [Theory, AutoNSubstituteData]
        public void Should_Be_EqualTo_With_Long(
            [Frozen] long version,
            StreamVersion sut)
            => (sut == version)
                .Should()
                .BeTrue();

        [Theory, AutoNSubstituteData]
        public void Should_Be_GreatherThan(
            [Frozen] long version,
            StreamVersion sut)
            => (sut < version + 1)
                .Should()
                .BeTrue();

        [Theory, AutoNSubstituteData]
        public void Should_Be_LessThan(
            [Frozen] long version,
            StreamVersion sut)
            => (sut > version - 1)
                .Should()
                .BeTrue();

        [Theory, AutoNSubstituteData]
        public void Should_Be_GreatherThan_Or_EqualTo(
            [Frozen] long version,
            StreamVersion sut)
            => (sut <= version + 1)
                .Should()
                .BeTrue();

        [Theory, AutoNSubstituteData]
        public void Should_Be_LessThan_Or_EqualTo(
            [Frozen] long version,
            StreamVersion sut)
            => (sut >= version - 1)
                .Should()
                .BeTrue();

        [Theory, AutoNSubstituteData]
        public void Should_Be_EqualTo_Using_CompareTo(
            [Frozen] long version,
            StreamVersion sut)
            => sut
                .CompareTo(version)
                .Should()
                .Be(0);

        [Theory, AutoNSubstituteData]
        public void Should_Be_GreatherThan_Using_CompareTo(
            [Frozen] long version,
            StreamVersion sut)
            => sut
                .CompareTo(version + 1)
                .Should()
                .Be(1);

        [Theory, AutoNSubstituteData]
        public void Should_Be_LessThan_Using_CompareTo(
            [Frozen] long version,
            StreamVersion sut)
            => sut
                .CompareTo(version - 1)
                .Should()
                .Be(-1);
    }
}