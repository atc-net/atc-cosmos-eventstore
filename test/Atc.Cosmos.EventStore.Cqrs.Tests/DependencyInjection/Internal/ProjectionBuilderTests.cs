namespace Atc.Cosmos.EventStore.Cqrs.Tests.DependencyInjection.Internal;

public sealed class ProjectionBuilderTests
{
    [Theory, AutoNSubstituteData]
    internal void Should_Set_Name(
        string name,
        ProjectionOptions options,
        ProjectionBuilder sut)
    {
        // Arrange
        sut.WithJobName(name);

        // Act
        sut.Build<TestProjection>(options);

        // Assert
        options.Name.Should().Be(name);
    }

    [Theory, AutoNSubstituteData]
    internal void ShouldThrow_When_Projection_IsMissing_ProjectionFilter(
        ProjectionOptions options,
        ProjectionBuilder sut)
    {
        // Act
        var act = () => sut.Build<TestProjectionMissingFilterAttribute>(options);

        // Assert
        act.Should().ThrowExactly<InvalidOperationException>();
    }

    [Theory, AutoNSubstituteData]
    internal void Should_Set_ExceptionHandler(
        ProcessExceptionHandler handler,
        ProjectionOptions options,
        ProjectionBuilder sut)
    {
        // Arrange
        sut.WithExceptionHandler(handler);

        // Act
        sut.Build<TestProjection>(options);

        // Assert
        options.ExceptionHandler.Should().Be(handler);
    }

    [Theory, AutoNSubstituteData]
    internal void Should_Have_Default_ExceptionHandler(
        ProjectionOptions options,
        ProjectionBuilder sut)
    {
        // Act
        sut.Build<TestProjection>(options);

        // Assert
        options.ExceptionHandler.Should().NotBeNull();
    }

    [Theory, AutoNSubstituteData]
    internal void Should_Set_StartFrom(
        SubscriptionStartOptions startFrom,
        ProjectionOptions options,
        ProjectionBuilder sut)
    {
        // Arrange
        sut.WithProjectionStartsFrom(startFrom);

        // Act
        sut.Build<TestProjection>(options);

        // Assert
        options.StartsFrom.Should().Be(startFrom);
    }

    [Theory, AutoNSubstituteData]
    internal void Should_Have_Default_StartFrom(
        ProjectionOptions options,
        ProjectionBuilder sut)
    {
        // Act
        sut.Build<TestProjection>(options);

        // Assert
        options.StartsFrom.Should().Be(SubscriptionStartOptions.FromBeginning);
    }

    [Theory, AutoNSubstituteData]
    internal void Should_Set_PollingInterval(
        TimeSpan pollingInterval,
        ProjectionOptions options,
        ProjectionBuilder sut)
    {
        // Arrange
        sut.WithPollingInterval(pollingInterval);

        // Act
        sut.Build<TestProjection>(options);

        // Assert
        options.PollingInterval.Should().Be(pollingInterval);
    }
}