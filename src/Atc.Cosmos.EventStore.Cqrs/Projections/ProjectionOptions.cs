using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Cqrs.Projections
{
    public class ProjectionOptions : IProjectionOptions
    {
        public static readonly ProcessExceptionHandler EmptyExceptionHandler = (e, ct)
            => new ValueTask(Task.CompletedTask);

        public ProjectionOptions()
        {
            Name = string.Empty;
            Filters = Array.Empty<StreamFilter>();
            ExceptionHandler = EmptyExceptionHandler;
        }

        public string Name { get; set; }

        public IReadOnlyCollection<StreamFilter> Filters { get; set; }

        public ProcessExceptionHandler ExceptionHandler { get; set; }
    }
}