using System.Text.Json;
using System.Text.Json.Serialization;

namespace Atc.Cosmos.EventStore.Converters;

internal class StreamVersionConverter : JsonConverter<StreamVersion>
{
    public override StreamVersion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TryGetInt64(out var value)
            ? value
            : throw new JsonException();

    public override void Write(Utf8JsonWriter writer, StreamVersion value, JsonSerializerOptions options)
        => writer.WriteNumberValue(value.Value);
}