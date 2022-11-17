using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;

namespace Atc.Cosmos.EventStore.Converters;

internal class EventDataConverterPipelineBuilder
{
    private readonly Collection<IEventDataConverter> converters = new();

    public EventDataConverterPipelineBuilder AddConverter(
        IEventDataConverter converter)
    {
        converters.Add(converter);

        return this;
    }

    public EventDataConverterPipelineBuilder AddConverters(
        IEnumerable<IEventDataConverter> converters)
    {
        foreach (var converter in converters)
        {
            this.converters.Add(converter);
        }

        return this;
    }

    public EventDataConverterPipeline Build()
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

        return new EventDataConverterPipeline(next);
    }
}