namespace GettingStarted.Events;

[StreamEvent("added-event:v1")]
public record AddedEvent(string Name, string Address);