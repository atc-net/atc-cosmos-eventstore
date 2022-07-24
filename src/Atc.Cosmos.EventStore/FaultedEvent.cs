using System;

namespace Atc.Cosmos.EventStore;

/// <summary>
/// Represents a faulted event read from the stream.
/// </summary>
/// <remarks>Inspect json and exception info to gain more insights.</remarks>
/// <param name="Json">Json that failed to deserialize.</param>
/// <param name="Exception">Exception details on failure.</param>
public record FaultedEvent(
    string Json,
    Exception? Exception);
