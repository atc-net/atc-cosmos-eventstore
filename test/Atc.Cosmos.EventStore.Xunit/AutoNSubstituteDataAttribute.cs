using Atc.Cosmos.EventStore.Xunit.Customizations;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;

namespace BigBang.Cosmos.EventStore.Xunit
{
    public class AutoNSubstituteDataAttribute : AutoDataAttribute
    {
        public AutoNSubstituteDataAttribute()
            : base(CreateCustomizedFixture)
        { }

        private static IFixture CreateCustomizedFixture()
        {
            var fixture = new Fixture();
            fixture.Customizations.Add(new CancellationTokenGenerator());

            return fixture.Customize(new AutoNSubstituteCustomization());
        }
    }
}