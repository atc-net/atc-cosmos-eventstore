using Atc.Cosmos.EventStore.Cqrs.Projections;
using Atc.Test;
using FluentAssertions;
using Xunit;

namespace Atc.Cosmos.EventStore.Cqrs.Tests.Projections
{
    public class ProjectionFilterTests
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
        public void Should_Evaluate_Using_One_Wildcard(
            string filter,
            string streamId,
            bool pass)
            => new ProjectionFilter(filter)
                .Evaluate(streamId)
                .Should()
                .Be(pass);

        [Theory]
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
        public void Should_Evaluate_All(
            string filter,
            string streamId,
            bool pass)
            => new ProjectionFilter(filter)
                .Evaluate(streamId)
                .Should()
                .Be(pass);
    }
}