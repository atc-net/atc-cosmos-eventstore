using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BigBang.Cosmos.EventStore
{
    /// <summary>
    /// Responsible for producing <see cref="Event"/>(s) from one or more event data objects.
    /// </summary>
    public class EventProducer
    {
        private readonly StreamType streamType;

        public EventProducer(StreamType streamType)
        {
            this.streamType = streamType;
        }

        /// <summary>
        /// Gets the partition key.
        /// </summary>
        /// <param name="streamName">Name of the stream.</param>
        /// <param name="streamId">Id of the stream.</param>
        /// <returns>Partition key string.</returns>
        public static string GetPartitionKey(string streamName, string streamId)
            => $"{streamName}_{streamId}";

        /// <summary>
        /// Creates a list <seealso cref="Event"/>s from a list of data object.
        /// </summary>
        /// <param name="events">List of event data objects to wrap in an <seealso cref="Event"/>.</param>
        /// <param name="streamId">Id of the stream this event will belong too.</param>
        /// <param name="streamName">Name of the stream this event will belong too.</param>
        /// <param name="fromVersion">Current version of the event stream.</param>
        /// <param name="correlationId">Optionally provide a correlation id or one will be automatically be created.</param>
        /// <returns>List of data objects wrapped in an <seealso cref="Event"/>.</returns>
        public IReadOnlyCollection<Event> Produce(
            IReadOnlyCollection<object> events,
            string streamId,
            string streamName,
            long fromVersion,
            string? correlationId = null)
        {
            // When producing a set of events we need them to contain the same
            // correlation id so we can later identify them as one batch.
            var cId = GetCorrelationId(correlationId);
            var timestamp = DateTimeOffset.UtcNow;

            return events
                .Select(data => CreateEvent(data, streamId, streamName, fromVersion++, cId, timestamp))
                .ToList();
        }

        /// <summary>
        /// Creates an <seealso cref="Event"/> from an event data object.
        /// </summary>
        /// <param name="eventData">Event data object to wrap in an <seealso cref="Event"/>.</param>
        /// <param name="streamId">Id of the stream this event will belong too.</param>
        /// <param name="streamName">Name of the stream this event will belong too.</param>
        /// <param name="fromVersion">Current version of the event stream.</param>
        /// <param name="correlationId">Optionally provide a correlation id or one will be automatically be created.</param>
        /// <returns>The data object wrapped in an <seealso cref="Event"/>.</returns>
        public Event Produce(object eventData, string streamId, string streamName, long fromVersion, string? correlationId = null)
            => CreateEvent(eventData, streamId, streamName, fromVersion, GetCorrelationId(correlationId), DateTimeOffset.UtcNow);

        private static string GetCorrelationId(string? correlationId)
            => correlationId ?? Guid.NewGuid().ToString();

        private static long GetVersion(long fromVersion)
            => fromVersion - 1;

        private static string GetETag(EventProperties properties)
            => Convert.ToBase64String(
                Encoding.ASCII.GetBytes(
                    $"{properties.StreamName}/{properties.StreamId}/{properties.Version}"));

        private Event CreateEvent(
            object eventData,
            string streamId,
            string streamName,
            long fromVersion,
            string correlationId,
            DateTimeOffset timestamp)
        {
            var evt = new Event<object>(eventData, new EventProperties
            {
                CorrelationId = GetCorrelationId(correlationId),
                StreamId = streamId,
                StreamName = streamName,
                Version = GetVersion(fromVersion),
                Timestamp = timestamp,
            });
            evt.Id = GetDocumentId(evt.Properties);
            evt.PartitionKey = GetPartitionKey(streamName, streamId);
            evt.Properties.Etag = GetETag(evt.Properties);

            return evt;
        }

        private string GetDocumentId(EventProperties properties)
            => streamType == StreamType.Timeseries
                ? $"{properties.StreamName}_{properties.StreamId}_{Guid.NewGuid()}"
                : $"{properties.StreamName}_{properties.StreamId}_{properties.Version}";
    }
}