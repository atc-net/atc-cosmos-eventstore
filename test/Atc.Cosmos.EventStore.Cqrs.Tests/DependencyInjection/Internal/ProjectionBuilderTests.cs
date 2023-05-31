using Atc.Cosmos.EventStore.Cqrs.DependencyInjection.Internal;
using Atc.Cosmos.EventStore.Cqrs.Projections;
using Atc.Cosmos.EventStore.Cqrs.Tests.Mocks;
using Atc.Test;
using FluentAssertions;
using Xunit;

namespace Atc.Cosmos.EventStore.Cqrs.Tests.DependencyInjection.Internal;

public class ProjectionBuilderTests
{
    [Theory, AutoNSubstituteData]
    internal void Should_Set_Name(
        string name,
        ProjectionOptions options,
        ProjectionBuilder sut)
    {
        sut.WithJobName(name);
        sut.Build<TestProjection>(options);

        options.Name.Should().Be(name);
    }

    [Theory, AutoNSubstituteData]
    internal void ShouldThrow_When_Projection_IsMissing_ProjectionFilter(
        ProjectionOptions options,
        ProjectionBuilder sut)
        => FluentActions
            .Invoking(() => sut.Build<TestProjectionMissingFilterAttribute>(options))
            .Should()
            .ThrowExactly<ArgumentException>();

    [Theory, AutoNSubstituteData]
    internal void Should_Set_ExceptionHandler(
        ProcessExceptionHandler handler,
        ProjectionOptions options,
        ProjectionBuilder sut)
    {
        sut.WithExceptionHandler(handler);
        sut.Build<TestProjection>(options);

        options.ExceptionHandler.Should().Be(handler);
    }

    [Theory, AutoNSubstituteData]
    internal void ShouldHave_Default_ExceptionHandler(
        ProjectionOptions options,
        ProjectionBuilder sut)
    {
        sut.Build<TestProjection>(options);

        options.ExceptionHandler.Should().NotBeNull();
    }
}
