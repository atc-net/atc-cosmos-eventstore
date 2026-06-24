#pragma warning disable ATC220 // Using directives should be moved to GlobalUsings.cs
#pragma warning disable ATC221 // Using directives should be moved to GlobalUsings.cs
using System.IO;
#pragma warning restore ATC221
#pragma warning restore ATC220

namespace Atc.Cosmos.EventStore.Cosmos;

/// <summary>
/// EventStore cosmos JSON serializer implementation for <seealso cref="System.Text.Json"/>.
/// </summary>
internal class CosmosEventSerializer : CosmosSerializer
{
    private readonly JsonSerializerOptions jsonSerializerOptions;

    public CosmosEventSerializer(
        IOptions<EventStoreClientOptions> options,
        IEventTypeProvider typeProvider)
    {
        jsonSerializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        jsonSerializerOptions.Converters.Add(new TimeSpanConverter());
        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        jsonSerializerOptions.Converters.Add(new StreamIdConverter());
        jsonSerializerOptions.Converters.Add(new StreamVersionConverter());
        jsonSerializerOptions.Converters.Add(
            new EventDocumentConverter(
                new EventDataConverterPipelineBuilder()
                    .AddConverter(new FaultedEventDataConverter())
                    .AddConverter(new UnknownEventDataConverter())
                    .AddConverters(options.Value.EventDataConverter)
                    .AddConverter(new NamedEventConverter(typeProvider))
                    .Build()));

        foreach (var converter in options.Value.CustomJsonConverter)
        {
            jsonSerializerOptions.Converters.Add(converter);
        }
    }

    [return: MaybeNull]
    public override T FromStream<T>(Stream stream)
    {
        if (stream is null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        using (stream)
        {
            if (stream is { CanSeek: true, Length: 0 })
            {
                return default;
            }

            if (typeof(Stream).IsAssignableFrom(typeof(T)))
            {
                return (T)(object)stream;
            }

            // Fast path: response data from cosmos usually comes as a memory
            // stream whose buffer can be read directly without copying.
            if (stream is MemoryStream memoryStream && memoryStream.TryGetBuffer(out var buffer))
            {
                return JsonSerializer.Deserialize<T>(buffer, jsonSerializerOptions);
            }

            // The Cosmos SDK does not guarantee the response is a MemoryStream
            // with a publicly visible buffer (this changed in newer SDK
            // versions), so fall back to copying the stream into a buffer.
            using var copy = new MemoryStream();
            stream.CopyTo(copy);

            return JsonSerializer.Deserialize<T>(
                new ReadOnlySpan<byte>(copy.GetBuffer(), 0, (int)copy.Length),
                jsonSerializerOptions);
        }
    }

    public override Stream ToStream<T>(T input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        var streamPayload = new MemoryStream();

        using var utf8JsonWriter = new Utf8JsonWriter(
            streamPayload,
            new JsonWriterOptions
            {
                Indented = jsonSerializerOptions.WriteIndented,
            });

        JsonSerializer.Serialize(utf8JsonWriter, input, jsonSerializerOptions);
        streamPayload.Position = 0;

        return streamPayload;
    }
}