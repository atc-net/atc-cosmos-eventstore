using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface IEventCatalogBuilder
    {
        IEventCatalogBuilder FromType(string name, Type type);
    }
}