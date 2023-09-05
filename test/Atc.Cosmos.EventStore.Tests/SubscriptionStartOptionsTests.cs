using Atc.Test;
using FluentAssertions;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests;

public class SubscriptionStartOptionsTests
{
    [Theory, AutoNSubstituteData]
    public void Should_Be_DefaultConstructed_With_StartFromBeginning(
        SubscriptionStartOptions sut)
        => sut
            .Should()
            .Be(SubscriptionStartOptions.FromBegining);

    [Theory, AutoNSubstituteData]
    public void Should_Be_EqualTo(
        SubscriptionStartOptions left,
        SubscriptionStartOptions right)
        => (left == right)
            .Should()
            .BeTrue();

    [Theory, AutoNSubstituteData]
    public void ShouldNot_Be_EqualTo(
        SubscriptionStartOptions right)
        => (SubscriptionStartOptions.FromDateTime(DateTime.Now) == right)
            .Should()
            .BeFalse();

    [Theory, AutoNSubstituteData]
    public void Should_Set_StartFrom_As_UniversalTime(
        DateTime startFrom)
        => SubscriptionStartOptions
            .FromDateTime(startFrom)
            .StartFrom
            .Should()
            .Be(startFrom.ToUniversalTime());
}