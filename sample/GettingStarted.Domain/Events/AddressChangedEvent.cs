namespace GettingStarted.Events;

[StreamEvent("address-changed-event:v1")]
public record AddressChangedEvent(string OldAddress, string NewAddress);