namespace Atc.Cosmos.EventStore;

/// <summary>
/// Represents an unknown event read from stream.
/// </summary>
/// <remarks>Inspect metadata to identify the unknown event name.</remarks>
/// <param name="Json">Event data json.</param>
public record UnknownEvent(
    string Json);
