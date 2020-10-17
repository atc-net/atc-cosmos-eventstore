using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BigBang.Cosmos.EventStore.Cqrs
{
    /// <summary>
    /// Responsible for invoking an interface method on a projection for a given event.
    /// </summary>
    public static class ProjectionInvoker
    {
        private static readonly ConcurrentDictionary<string, MethodInfo> methods
            = new ConcurrentDictionary<string, MethodInfo>();

        public static void ApplyProjection(this IReadOnlyCollection<Event> events, object projection, object model)
        {
            foreach (var @event in events)
            {
                @event.ApplyProjection(projection, model);
            }
        }

        public static void ApplyProjection(this Event @event, object projection, object model)
        {
            var key = $"{@event.Data.GetType().FullName}-{model.GetType().FullName}";
            var action = methods.GetOrAdd(
                key,
                type => typeof(ProjectionInvoker)
                    .GetRuntimeMethods()
                    .First(m => m.Name.Equals(nameof(ProjectTypedEvent), StringComparison.InvariantCultureIgnoreCase))
                    .MakeGenericMethod(model.GetType(), @event.Data.GetType()));

            // Project event as a strongly type event on projection.
            action.Invoke(null, new object[] { projection, model, @event.Data, @event.Properties });
        }

        private static void ProjectTypedEvent<TModel, TEvent>(object projection, TModel model, TEvent evt, EventProperties properties)
            where TModel : class, new()
            where TEvent : class, new()
            => (projection as IEventProjection<TModel, TEvent>)?.ApplyEvent(model, evt, properties);
    }
}