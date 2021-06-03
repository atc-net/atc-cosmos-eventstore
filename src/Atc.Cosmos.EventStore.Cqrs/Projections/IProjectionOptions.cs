using System.Collections.Generic;

namespace Atc.Cosmos.EventStore.Cqrs.Projections
{
    public interface IProjectionOptions
    {
        string Name { get; }

        IReadOnlyCollection<StreamFilter> Filters { get; }

        ProcessExceptionHandler ExceptionHandler { get; }
    }
}