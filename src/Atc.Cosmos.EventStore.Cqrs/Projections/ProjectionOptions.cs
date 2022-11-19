namespace Atc.Cosmos.EventStore.Cqrs.Projections;

internal class ProjectionOptions : IProjectionOptions
{
    public static readonly ProcessExceptionHandler EmptyExceptionHandler = (e, ct)
        => Task.CompletedTask;

    public ProjectionOptions()
    {
        Name = string.Empty;
        Filters = Array.Empty<ProjectionFilter>();
        ExceptionHandler = EmptyExceptionHandler;
    }

    public string Name { get; set; }

    public IReadOnlyCollection<ProjectionFilter> Filters { get; set; }

    public ProcessExceptionHandler ExceptionHandler { get; set; }
}