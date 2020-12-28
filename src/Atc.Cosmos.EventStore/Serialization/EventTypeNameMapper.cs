using System;
using System.Collections.Generic;
using System.Linq;

namespace BigBang.Cosmos.EventStore.Serialization
{
    public class EventTypeNameMapper : IEventTypeNameMapper
    {
        private readonly IReadOnlyDictionary<string, Type> nameToType;
        private readonly IReadOnlyDictionary<Type, string> typeToName;

        public EventTypeNameMapper(IReadOnlyDictionary<Type, string> typeToName)
        {
            this.typeToName = typeToName;
            nameToType = GetNameToTypeMappings(typeToName);
        }

        public string GetEventName(Type type)
            => typeToName.ContainsKey(type)
                ? typeToName[type]
                : throw new ArgumentOutOfRangeException(nameof(type), $"Event type '{type?.Name ?? "null"}' is not registered with converter");

        public Type GetEventType(string name)
            => nameToType.ContainsKey(name)
                ? nameToType[name]
                : typeof(Event<object>);

        private static Type MakeEventType(Type type)
            => typeof(Event<>).MakeGenericType(type);

        private IReadOnlyDictionary<string, Type> GetNameToTypeMappings(IReadOnlyDictionary<Type, string> typeToName)
            => typeToName.ToDictionary(pair => pair.Value, pair => MakeEventType(pair.Key), StringComparer.Ordinal);
    }
}