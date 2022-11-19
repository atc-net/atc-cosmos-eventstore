namespace Atc.Cosmos.EventStore.Cqrs.Testing;

public interface ICommandContextInspector
{
    IReadOnlyCollection<object> Events { get; }

    object? ResponseObject { get; }
}