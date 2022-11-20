using System.Reflection;

namespace Atc.Cosmos.EventStore.Cqrs.Internal;

public abstract class ConsumeEventMetadata
{
    private readonly Dictionary<Type, MethodInfo> consumeEvents;
    private readonly bool canConsumeAnyEvent;

    protected ConsumeEventMetadata(
        Type type)
    {
        consumeEvents = type
            .GetInterfaces()
            .Where(t => t.IsGenericType)
            .Where(t => ImplementsConsumeEventInterfaces(t.GetGenericTypeDefinition()))
            .Select(t => t.GetGenericArguments()[0])
            .Distinct()
            .ToDictionary(
                t => t,
                t => typeof(ConsumeEventMetadata)
                        .GetRuntimeMethods()
                        .First(m => m.Name.Equals(nameof(ProjectTypedEvent), StringComparison.OrdinalIgnoreCase))
                        .MakeGenericMethod(t));

        canConsumeAnyEvent = type
            .GetInterfaces()
            .Any(t => t.UnderlyingSystemType == typeof(IConsumeAnyEvent)
                   || t.UnderlyingSystemType == typeof(IConsumeAnyEventAsync));
    }

    public bool CanConsumeEvent(IEvent evt)
        => consumeEvents
            .ContainsKey(evt.Data.GetType())
        || canConsumeAnyEvent;

    public bool IsNotConsumingEvents()
        => consumeEvents.Keys.Count == 0
        && !canConsumeAnyEvent;

    protected async ValueTask ConsumeAsync(
        IEvent evt,
        object projection,
        CancellationToken cancellationToken)
    {
        if (!CanConsumeEvent(evt))
        {
            return;
        }

        var metadata = new EventMetadata(
            evt.Metadata.EventId,
            EventStreamId.FromStreamId(evt.Metadata.StreamId),
            evt.Metadata.Timestamp,
            (long)evt.Metadata.Version,
            CorrelationId: evt.Metadata.CorrelationId,
            CausationId: evt.Metadata.CausationId);

        if (consumeEvents.TryGetValue(evt.Data.GetType(), out var method))
        {
            var response = method.Invoke(null, new object[] { projection, evt.Data, metadata, cancellationToken });

            if (response is ValueTask v)
            {
                await v.ConfigureAwait(false);
            }
        }

        if (canConsumeAnyEvent)
        {
            (projection as IConsumeAnyEvent)?.Consume(evt.Data, metadata);

            if (projection is IConsumeAnyEventAsync consumeAsync)
            {
                await consumeAsync
                    .ConsumeAsync(evt.Data, metadata, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }

    private static bool ImplementsConsumeEventInterfaces(Type genericTypeDefinition)
        => genericTypeDefinition == typeof(IConsumeEvent<>)
        || genericTypeDefinition == typeof(IConsumeEventAsync<>);

    private static async ValueTask ProjectTypedEvent<TEvent>(
        object projection,
        TEvent evt,
        EventMetadata metadata,
        CancellationToken cancellationToken)
        where TEvent : class
    {
        (projection as IConsumeEvent<TEvent>)?.Consume(evt, metadata);

        if (projection is IConsumeEventAsync<TEvent> consumeAsync)
        {
            await consumeAsync
                .ConsumeAsync(evt, metadata, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
