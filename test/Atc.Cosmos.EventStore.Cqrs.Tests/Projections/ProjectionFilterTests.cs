namespace Atc.Cosmos.EventStore.Cqrs.Tests.Projections;

public sealed class ProjectionFilterTests
{
    [Theory]
    [InlineAutoNSubstituteData("*", "type.1", false)]
    [InlineAutoNSubstituteData("*", "type.1.2", false)]
    [InlineAutoNSubstituteData("*", "type.1.2.3", false)]
    [InlineAutoNSubstituteData("*", "type.1.2.3.4", false)]
    [InlineAutoNSubstituteData("*", "type.1.2.3.4.5", false)]
    [InlineAutoNSubstituteData("*", "type", true)]
    [InlineAutoNSubstituteData("type.*", "type.1", true)]
    [InlineAutoNSubstituteData("type.*", "type.1.2", false)]
    [InlineAutoNSubstituteData("type.*", "type.1.2.3", false)]
    [InlineAutoNSubstituteData("type.*", "type.1.2.3.4", false)]
    [InlineAutoNSubstituteData("type.*", "type.1.2.3.4.5", false)]
    [InlineAutoNSubstituteData("type.*", "type", false)]
    [InlineAutoNSubstituteData("type.*.2", "type.1", false)]
    [InlineAutoNSubstituteData("type.*.2", "type.1.2", true)]
    [InlineAutoNSubstituteData("type.*.2", "type.1.2.3", false)]
    [InlineAutoNSubstituteData("type.*.2", "type.1.2.3.4", false)]
    [InlineAutoNSubstituteData("type.*.2", "type.1.2.3.4.5", false)]
    [InlineAutoNSubstituteData("type.*.*", "type", false)]
    [InlineAutoNSubstituteData("type.*.*", "type.1", false)]
    [InlineAutoNSubstituteData("type.*.*", "type.1.2", true)]
    [InlineAutoNSubstituteData("type.*.*", "type.1.2.3", false)]
    [InlineAutoNSubstituteData("type.*.*", "type.1.2.3.4", false)]
    [InlineAutoNSubstituteData("type.*.*", "type.1.2.3.4.5", false)]
    [InlineAutoNSubstituteData("type.*.*", "type", false)]
    [InlineAutoNSubstituteData("**", "type.1", true)]
    [InlineAutoNSubstituteData("**", "type.1.2", true)]
    [InlineAutoNSubstituteData("**", "type.1.2.3", true)]
    [InlineAutoNSubstituteData("**", "type.1.2.3.4", true)]
    [InlineAutoNSubstituteData("**", "type.1.2.3.4.5", true)]
    [InlineAutoNSubstituteData("**", "type", true)]
    [InlineAutoNSubstituteData("type.**", "type.1", true)]
    [InlineAutoNSubstituteData("type.**", "type.1.2", true)]
    [InlineAutoNSubstituteData("type.**", "type.1.2.3", true)]
    [InlineAutoNSubstituteData("type.**", "type.1.2.3.4", true)]
    [InlineAutoNSubstituteData("type.**", "type.1.2.3.4.5", true)]
    [InlineAutoNSubstituteData("type.**", "type", false)]
    [InlineAutoNSubstituteData("type.*.**", "type.1", false)]
    [InlineAutoNSubstituteData("type.*.**", "type.1.2", true)]
    [InlineAutoNSubstituteData("type.*.**", "type.1.2.3", true)]
    [InlineAutoNSubstituteData("type.*.**", "type.1.2.3.4", true)]
    [InlineAutoNSubstituteData("type.*.**", "type.1.2.3.4.5", true)]
    [InlineAutoNSubstituteData("type.*.*.**", "type", false)]
    [InlineAutoNSubstituteData("type.*.*.**", "type.1", false)]
    [InlineAutoNSubstituteData("type.*.*.**", "type.1.2", false)]
    [InlineAutoNSubstituteData("type.*.*.**", "type.1.2.3", true)]
    [InlineAutoNSubstituteData("type.*.*.**", "type.1.2.3.4", true)]
    [InlineAutoNSubstituteData("type.*.*.**", "type.1.2.3.4.5", true)]
    [InlineAutoNSubstituteData("type.*.*.**", "type", false)]
    public void Should_Evaluate_Filter(
        string filter,
        string streamId,
        bool pass)
    {
        // Act
        var result = new ProjectionFilter(filter).Evaluate(streamId);

        // Assert
        result.Should().Be(pass);
    }
}