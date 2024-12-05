using Atc.Cosmos.EventStore.Cqrs;

namespace GettingStarted;

[StreamEvent("name-changed-event:v1")]
public record NameChangedEvent(string OldName, string NewName);