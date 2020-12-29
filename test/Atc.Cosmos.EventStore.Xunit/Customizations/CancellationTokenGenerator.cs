using System.Reflection;
using System.Threading;
using AutoFixture.Kernel;

namespace Atc.Cosmos.EventStore.Xunit.Customizations
{
    public class CancellationTokenGenerator : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var pi = request as ParameterInfo;
            if (pi is null)
            {
                return new NoSpecimen();
            }

            if (pi.ParameterType == typeof(CancellationToken))
            {
                return new CancellationToken(false);
            }

            return new NoSpecimen();
        }
    }
}