namespace GettingStarted.Events;

[StreamEvent("deleted-event:v1")]
public record DeletedEvent(string Reason);