using System;
using System.Text.Json;

namespace Atc.Cosmos.EventStore.Converters;

internal class FaultedEventDataConverter : IEventDataConverter
{
    public object? Convert(
        IEventMetadata metadata,
        JsonElement data,
        JsonSerializerOptions options,
        Func<object?> next)
    {
        try
        {
            return next.Invoke();
        }
        catch (Exception ex)
        {
            return new FaultedEvent(
                data.GetRawText(),
                ex);
        }
    }
}
