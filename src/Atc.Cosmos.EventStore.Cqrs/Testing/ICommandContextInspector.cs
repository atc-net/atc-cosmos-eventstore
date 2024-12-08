namespace Atc.Cosmos.EventStore.Cqrs.Testing;

public interface ICommandContextInspector
{
    StreamVersion StreamVersion { get; }

    IReadOnlyCollection<object> Events { get; }

    object? ResponseObject { get; }
}