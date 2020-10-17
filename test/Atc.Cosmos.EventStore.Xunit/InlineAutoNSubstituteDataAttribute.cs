using AutoFixture.Xunit2;

namespace BigBang.Cosmos.EventStore.Xunit
{
    public class InlineAutoNSubstituteDataAttribute : InlineAutoDataAttribute
    {
        public InlineAutoNSubstituteDataAttribute(params object[] values)
            : base(new AutoNSubstituteDataAttribute(), values) { }
    }
}