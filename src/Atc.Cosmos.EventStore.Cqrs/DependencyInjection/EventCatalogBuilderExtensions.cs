using System.Reflection;
using Atc.Cosmos.EventStore.Cqrs;

namespace Microsoft.Extensions.DependencyInjection;

public static class EventCatalogBuilderExtensions
{
    /// <summary>
    /// Search assembly for events and register them in catalog.
    /// </summary>
    /// <remarks>Only classes marked with <seealso cref="StreamEventAttribute"/> will be added to the catalog.</remarks>
    /// <typeparam name="T">Use this type to identify the assembly to scan.</typeparam>
    /// <param name="builder">Catalog build to add events to.</param>
    /// <returns>Builder.</returns>
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

    /// <summary>
    /// Search namespace for events and register them in catalog.
    /// </summary>
    /// <remarks>Only classes marked with <seealso cref="StreamEventAttribute"/> will be added to the catalog.</remarks>
    /// <typeparam name="T">Use this types namespace.</typeparam>
    /// <param name="builder">Catalog build to add events to.</param>
    /// <returns>Builder.</returns>
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

    /// <summary>
    /// Add event to catalog.
    /// </summary>
    /// <typeparam name="T">Type to add.</typeparam>
    /// <param name="builder">Catalog to add type to.</param>
    /// <returns>Builder.</returns>
    /// <exception cref="ArgumentException">If type is not decorated with <see cref="StreamEventAttribute"/>.</exception>
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