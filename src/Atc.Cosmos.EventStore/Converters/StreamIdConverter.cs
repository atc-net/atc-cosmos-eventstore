namespace Atc.Cosmos.EventStore.Converters;

internal class StreamIdConverter : JsonConverter<StreamId>
{
    public override StreamId Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return value ?? throw new JsonException();
    }

    public override void Write(
        Utf8JsonWriter writer,
        StreamId value,
        JsonSerializerOptions options)
        => writer.WriteStringValue(value.Value);
}