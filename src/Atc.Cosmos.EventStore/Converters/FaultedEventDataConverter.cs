using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Atc.Cosmos.EventStore.Converters;

internal class FaultedEventDataConverter : IEventDataConverter
{
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "ByDesign")]
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
