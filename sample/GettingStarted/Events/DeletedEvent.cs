using Atc.Cosmos.EventStore.Cqrs;

namespace GettingStarted;

[StreamEvent("deleted-event:v1")]
public record DeletedEvent(string Reason);