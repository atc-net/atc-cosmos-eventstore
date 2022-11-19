using System.Collections.ObjectModel;

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
        IEventDataConverterPipelineExecutor? pipeline = null;
        foreach (var converter in converters.Reverse())
        {
            pipeline = new EventDataConverterExecutor(converter, pipeline);
        }

        return new EventDataConverterPipeline(pipeline);
    }
}
