namespace Atc.Cosmos.EventStore.Cqrs;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class StreamEventAttribute : Attribute
{
    public StreamEventAttribute(string name)
    {
        Name = name;
    }

    public EventName Name { get; }
}