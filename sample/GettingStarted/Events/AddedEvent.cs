using Atc.Cosmos.EventStore.Cqrs;

namespace GettingStarted;

[StreamEvent("added-event:v1")]
public record AddedEvent(string Name, string Address);