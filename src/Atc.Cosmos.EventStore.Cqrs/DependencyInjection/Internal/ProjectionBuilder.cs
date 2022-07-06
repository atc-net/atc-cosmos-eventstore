using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public void Build<TProjection>(ProjectionOptions options)
            where TProjection : class, IProjection
        {
            SetFiltersFromProjection<TProjection>();
            if (filters.Count == 0)
            {
                throw new ArgumentException(
                    $"Please provide a filter for type {typeof(TProjection)}");
            }

            options.Name = name;
            options.Filters = filters;
            options.ExceptionHandler = exceptionHandler;
        }

        private void SetFiltersFromProjection<T>()
               => Array.ForEach(
                   typeof(T)
                       .GetCustomAttributes<ProjectionFilterAttribute>()
                       .Select(a => a.Filter)
                       .ToArray(),
                   f => WithFilter(f));
    }
}