using Atc.Cosmos.EventStore.Cqrs;

namespace GettingStarted;

[StreamEvent("address-changed-event:v1")]
public record AddressChangedEvent(string OldAddress, string NewAddress);