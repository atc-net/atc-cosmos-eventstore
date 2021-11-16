using System.Collections.Generic;

namespace Atc.Cosmos.EventStore.Cqrs.Projections
{
    internal interface IProjectionOptions
    {
        string Name { get; }

        IReadOnlyCollection<StreamFilter> Filters { get; }

        ProcessExceptionHandler ExceptionHandler { get; }
    }
}