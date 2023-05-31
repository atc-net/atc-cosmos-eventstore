using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Atc.Cosmos.EventStore.Converters;

/// <summary>
/// Represent a convert responsible for converting an event data json element into a CLR object.
/// </summary>
public interface IEventDataConverter
{
    /// <summary>
    /// Convert <see cref="JsonElement"/> to an object.
    /// </summary>
    /// <param name="metadata">Event metadata information.</param>
    /// <param name="data">Event data to convert.</param>
    /// <param name="options">Json serlization options to use.</param>
    /// <param name="next">Delegate for parsing the convertion on to the next in the pipeline.</param>
    /// <returns>The converted object or null.</returns>
    [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "ByDesign")]
    object? Convert(
        IEventMetadata metadata,
        JsonElement data,
        JsonSerializerOptions options,
        Func<object?> next);
}
