using Atc.Cosmos.EventStore.Cqrs.DependencyInjection.Internal;
using Atc.Cosmos.EventStore.Cqrs.Projections;
using Atc.Cosmos.EventStore.Cqrs.Tests.Mocks;
using Atc.Test;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
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
        var abstraction = sut as IProjectionBuilder;
        abstraction.WithJobName(name);
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
        var abstraction = sut as IProjectionBuilder;
        abstraction.WithExceptionHandler(handler);
        sut.Build<TestProjection>(options);

        options.ExceptionHandler.Should().Be(handler);
    }

    [Theory, AutoNSubstituteData]
    internal void Should_Have_Default_ExceptionHandler(
        ProjectionOptions options,
        ProjectionBuilder sut)
    {
        sut.Build<TestProjection>(options);

        options.ExceptionHandler.Should().NotBeNull();
    }

    [Theory, AutoNSubstituteData]
    internal void Should_Set_StartFrom(
        SubscriptionStartOptions startFrom,
        ProjectionOptions options,
        ProjectionBuilder sut)
    {
        var abstraction = sut as IProjectionBuilder;
        abstraction.WithProjectionStartsFrom(startFrom);
        sut.Build<TestProjection>(options);

        options.StartsFrom.Should().Be(startFrom);
    }

    [Theory, AutoNSubstituteData]
    internal void Should_Have_Default_StartFrom(
        ProjectionOptions options,
        ProjectionBuilder sut)
    {
        sut.Build<TestProjection>(options);

        options.StartsFrom.Should().Be(SubscriptionStartOptions.FromBegining);
    }

    [Theory, AutoNSubstituteData]
    internal void Should_Set_PollingInterval(
        TimeSpan pollingInterval,
        ProjectionOptions options,
        ProjectionBuilder sut)
    {
        var abstraction = sut as IProjectionBuilder;
        abstraction.WithPollingInterval(pollingInterval);
        sut.Build<TestProjection>(options);

        options.PollingInterval.Should().Be(pollingInterval);
    }
}
