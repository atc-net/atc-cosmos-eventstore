namespace GettingStarted.Events;

[StreamEvent("name-changed-event:v1")]
public record NameChangedEvent(string OldName, string NewName);