using System.Text.Json;

namespace Atc.Cosmos.EventStore.Converters;

/// <summary>
/// Responsible for converting the data to raw json string.
/// </summary>
internal class UnknownEventDataConverter : IEventDataConverter
{
    public object? Convert(
        IEventMetadata metadata,
        JsonElement data,
        JsonSerializerOptions options,
        Func<object?> next)
        => next.Invoke()
        switch
        {
            { } converted => converted,
            _ => new UnknownEvent(data.GetRawText()),
        };
}
