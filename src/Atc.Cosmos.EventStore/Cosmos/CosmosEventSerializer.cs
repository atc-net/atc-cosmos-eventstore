using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Atc.Cosmos.EventStore.Converters;
using Atc.Cosmos.EventStore.Events;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Atc.Cosmos.EventStore.Cosmos
{
    /// <summary>
    /// EventStore cosmos JSON serializer implementation for System.Text.Json.
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

            jsonSerializerOptions.Converters.Add(new EventDocumentConverter(typeProvider));
            jsonSerializerOptions.Converters.Add(new TimeSpanConverter());
            jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            jsonSerializerOptions.Converters.Add(new StreamIdConverter());
            jsonSerializerOptions.Converters.Add(new StreamVersionConverter());

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
                if (stream.CanSeek && stream.Length == 0)
                {
                    return default;
                }

                if (typeof(Stream).IsAssignableFrom(typeof(T)))
                {
                    return (T)(object)stream;
                }

                // Response data from cosmos always comes as a memory stream.
                // Note: This might change in v4, but so far it doesn't look like it.
                if (stream is MemoryStream memoryStream && memoryStream.TryGetBuffer(out ArraySegment<byte> buffer))
                {
                    return JsonSerializer.Deserialize<T>(buffer, jsonSerializerOptions);
                }

                return default;
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
}