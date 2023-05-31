using System.Text.Json;
using System.Text.Json.Serialization;

namespace Atc.Cosmos.EventStore.Converters;

internal class StreamIdConverter : JsonConverter<StreamId>
{
    public override StreamId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (value is null)
        {
            throw new JsonException();
        }

        return value;
    }

    public override void Write(Utf8JsonWriter writer, StreamId value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Value);
}