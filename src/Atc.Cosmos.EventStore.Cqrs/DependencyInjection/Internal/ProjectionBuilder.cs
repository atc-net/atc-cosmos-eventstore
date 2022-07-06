using System.Collections.Generic;
using Atc.Cosmos.EventStore.Cqrs.Projections;
using Microsoft.Extensions.DependencyInjection;

namespace Atc.Cosmos.EventStore.Cqrs.DependencyInjection.Internal
{
    internal class ProjectionBuilder : IProjectionBuilder
    {
        private readonly List<ProjectionFilter> filters;
        private ProcessExceptionHandler exceptionHandler;
        private string name;

        public ProjectionBuilder(string name)
        {
            this.name = name;
            filters = new List<ProjectionFilter>();
            exceptionHandler = ProjectionOptions.EmptyExceptionHandler;
        }

        public IProjectionBuilder WithFilter(string filter)
        {
            filters.Add(new ProjectionFilter(filter));

            return this;
        }

        public IProjectionBuilder WithJobName(string name)
        {
            this.name = name;

            return this;
        }

        public IProjectionBuilder WithExceptionHandler(ProcessExceptionHandler handler)
        {
            this.exceptionHandler = handler;

            return this;
        }

        public void Build(ProjectionOptions options)
        {
            options.Name = name;
            options.Filters = filters;
            options.ExceptionHandler = exceptionHandler;
        }
    }
}