namespace Atc.Cosmos.EventStore.Tests;

public sealed class SubscriptionStartOptionsTests
{
    [Theory, AutoNSubstituteData]
    public void Should_Be_DefaultConstructed_With_StartFromBeginning(
        SubscriptionStartOptions sut)
    {
        // Act
        var result = sut;

        // Assert
        result
            .Should()
            .Be(SubscriptionStartOptions.FromBegining);
    }

    [Theory, AutoNSubstituteData]
    public void Should_Be_EqualTo(
        SubscriptionStartOptions left,
        SubscriptionStartOptions right)
    {
        // Act
        var result = left == right;

        // Assert
        result
            .Should()
            .BeTrue();
    }

    [Theory, AutoNSubstituteData]
    public void ShouldNot_Be_EqualTo(SubscriptionStartOptions right)
    {
        // Act
        var result = SubscriptionStartOptions.FromDateTime(DateTime.Now) == right;

        // Assert
        result
            .Should()
            .BeFalse();
    }

    [Theory, AutoNSubstituteData]
    public void Should_Set_StartFrom_As_UniversalTime(DateTime startFrom)
    {
        // Act
        var result = SubscriptionStartOptions.FromDateTime(startFrom).StartFrom;

        // Assert
        result
            .Should()
            .Be(startFrom.ToUniversalTime());
    }
}