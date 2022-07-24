using System;
using System.Text.Json;
using Atc.Cosmos.EventStore.Events;

namespace Atc.Cosmos.EventStore.Converters;

/// <summary>
/// Responsible for converting from a named event to a CLI type.
/// </summary>
internal class NamedEventConverter : IEventDataConverter
{
    private readonly IEventTypeProvider typeProvider;

    public NamedEventConverter(
        IEventTypeProvider typeProvider)
        => this.typeProvider = typeProvider;

    public object? Convert(
        IEventMetadata metadata,
        JsonElement data,
        JsonSerializerOptions options,
        Func<object?> next)
        => typeProvider.GetEventType(metadata.Name)
        switch
        {
            { } type => data.Deserialize(type, options),
            _ => next.Invoke(),
        };
}