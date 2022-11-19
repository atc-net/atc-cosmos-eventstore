namespace Atc.Cosmos.EventStore.Cqrs;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class ProjectionFilterAttribute : Attribute
{
    public ProjectionFilterAttribute(
        string filter)
    {
        Filter = filter;
    }

    public string Filter { get; }
}