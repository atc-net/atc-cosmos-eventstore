using System.Reflection;
using Atc.Cosmos.EventStore.Cqrs.Projections;
using Microsoft.Extensions.DependencyInjection;

namespace Atc.Cosmos.EventStore.Cqrs.DependencyInjection.Internal;

internal class ProjectionBuilder : IProjectionBuilder
{
    private readonly List<ProjectionFilter> filters;
    private ProcessExceptionHandler exceptionHandler;
    private SubscriptionStartOptions startFrom;
    private string name;
    private TimeSpan pollingInterval;
    private int maxItems;

    public ProjectionBuilder(string name)
    {
        this.name = name;
        filters = new List<ProjectionFilter>();
        exceptionHandler = ProjectionOptions.EmptyExceptionHandler;
        startFrom = SubscriptionStartOptions.FromBegining;
        pollingInterval = TimeSpan.FromSeconds(1);
        maxItems = 100;
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

    public IProjectionBuilder WithProjectionStartsFrom(SubscriptionStartOptions startFrom)
    {
        this.startFrom = startFrom;

        return this;
    }

    public IProjectionBuilder WithPollingInterval(TimeSpan pollingInterval)
    {
        this.pollingInterval = pollingInterval;

        return this;
    }

    public IProjectionBuilder WithMaxItems(int maxItems)
    {
        this.maxItems = maxItems;

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
        options.StartsFrom = startFrom;
        options.PollingInterval = pollingInterval;
        options.MaxItems = maxItems;
        options.ShareProjectionAcrossProcesses = true;
    }

    private void SetFiltersFromProjection<T>()
           => Array.ForEach(
               typeof(T)
                   .GetCustomAttributes<ProjectionFilterAttribute>()
                   .Select(a => a.Filter)
                   .ToArray(),
               f => WithFilter(f));
}