using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Atc.Cosmos.EventStore.Converters;

internal class EventDataConverterPipeline
{
    private readonly Func<IEventMetadata, JsonElement, JsonSerializerOptions, object?> pipeline;

    public EventDataConverterPipeline(
        IEnumerable<IEventDataConverter> converters)
    {
        Func<IEventMetadata, JsonElement, JsonSerializerOptions, object?> next = (_, _, _) => null;
        foreach (var c in converters.Reverse())
        {
            next = (meta, data, options) => c.Convert(
                meta,
                data,
                options,
                () => next.Invoke(meta, data, options));
        }

        pipeline = next;
    }

    public object Convert(
        IEventMetadata metadata,
        JsonElement data,
        JsonSerializerOptions options)
        => pipeline.Invoke(metadata, data, options)
        ?? new UnknownEvent(
            data.GetRawText());
}