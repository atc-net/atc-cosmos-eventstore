namespace Atc.Cosmos.EventStore.Cqrs;

public interface ICommandContext
{
    StreamVersion StreamVersion { get; }

    void AddEvent(object evt);

    object? ResponseObject { get; set; }
}