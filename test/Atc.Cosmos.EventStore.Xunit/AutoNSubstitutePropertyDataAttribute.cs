using AutoFixture.Xunit2;
using Xunit;
using Xunit.Sdk;

namespace BigBang.Cosmos.EventStore.Xunit
{
    public class AutoNSubstitutePropertyDataAttribute : CompositeDataAttribute
    {
        public AutoNSubstitutePropertyDataAttribute(string propertyName)
            : base(
                  new DataAttribute[]
                  {
                      new MemberDataAttribute(propertyName),
                      new AutoNSubstituteDataAttribute(),
                  })
        {
        }
    }
}