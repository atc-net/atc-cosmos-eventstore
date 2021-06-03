using System;
using System.Linq;
using System.Reflection;
using Atc.Cosmos.EventStore.Cqrs;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventCatalogBuilderExtensions
    {
        public static IEventCatalogBuilder FromAssembly<T>(this IEventCatalogBuilder builder)
        {
            var types = typeof(T)
                .Assembly
                .GetTypes()
                .Where(t => t.GetCustomAttribute<StreamEventAttribute>() != null)
                .Select(t => new { Type = t, Name = t.GetCustomAttribute<StreamEventAttribute>()!.Name.Value })
                .ToArray();

            foreach (var type in types)
            {
                builder.FromType(type.Name, type.Type);
            }

            return builder;
        }

        public static IEventCatalogBuilder FromNamespace<T>(this IEventCatalogBuilder builder)
        {
            var ns = typeof(T)
                .Namespace;
            if (ns is null)
            {
                return builder;
            }

            var types = typeof(T)
                .Assembly
                .GetTypes()
                .Where(t => ns.Equals(t.Namespace, StringComparison.Ordinal))
                .Where(t => t.GetCustomAttribute<StreamEventAttribute>() != null)
                .Select(t => new { Type = t, Name = t.GetCustomAttribute<StreamEventAttribute>()!.Name.Value })
                .ToArray();

            foreach (var type in types)
            {
                builder.FromType(type.Name, type.Type);
            }

            return builder;
        }

        public static IEventCatalogBuilder FromType<T>(this IEventCatalogBuilder builder)
        {
            var name = typeof(T)
                .GetCustomAttribute<StreamEventAttribute>()
                ?.Name.Value;

            if (name is null)
            {
                throw new ArgumentException(
                    $"Type does not have {nameof(StreamEventAttribute)} declared on it.",
                    typeof(T).Name);
            }

            builder.FromType(name, typeof(T));

            return builder;
        }
    }
}