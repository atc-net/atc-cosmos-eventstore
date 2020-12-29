using System;
using System.Collections.Generic;
using System.Linq;

namespace BigBang.Cosmos.EventStore
{
    public static class EventExtensions
    {
        public static IReadOnlyCollection<PartitionedEvents> GroupByStreamId(this IReadOnlyCollection<Event> events)
            => events
                .GroupBy(e => e.Properties.StreamId, StringComparer.Ordinal)
                .Select(g => new PartitionedEvents(g.Key, g.ToList()))
                .ToList();

        /// <summary>
        /// Validate if <paramref name="name"/> only contains valid characters.
        /// </summary>
        /// <param name="name">Name to validate.</param>
        /// <returns>True if invalid characters is found, otherwise false.</returns>
        public static bool HasInvalidCharacters(this string name)
            => name.Contains("/", StringComparison.InvariantCulture)
            || name.Contains("\\", StringComparison.InvariantCulture)
            || name.Contains("?", StringComparison.InvariantCulture)
            || name.Contains("#", StringComparison.InvariantCulture);

        /// <summary>
        /// Throws an exception if validation for max length and invalid characters fails.
        /// </summary>
        /// <remarks>Max length is 64 and following characters are restricted and cannot be used: '/', '\\', '?', '#'.</remarks>
        /// <exception cref="ArgumentException">Is thrown if <paramref name="streamName"/> is invalid.</exception>
        /// <param name="streamName">Stream name to validate.</param>
        /// <returns>Returns the <paramref name="streamName"/> if valid, otherwise an <seealso cref="ArgumentException"/> is thrown.</returns>
        public static string ThrowIfStreamNameIsInvalid(this string streamName)
            => (streamName.Length > 64 || streamName.HasInvalidCharacters())
             ? throw new ArgumentException("Max length is 64 and following characters are restricted and cannot be used: '/', '\\', '?', '#'", nameof(streamName))
             : streamName;

        /// <summary>
        /// Throws an exception if validation for max length and invalid characters fails.
        /// </summary>
        /// <remarks>Max length is 64 and following characters are restricted and cannot be used: '/', '\\', '?', '#'.</remarks>
        /// <exception cref="ArgumentException">Is thrown if <paramref name="streamId"/> is invalid.</exception>
        /// <param name="streamId">Stream id to validate.</param>
        /// <returns>Returns the <paramref name="streamId"/> if valid, otherwise an <seealso cref="ArgumentException"/> is thrown.</returns>
        public static string ThrowIfStreamIdIsInvalid(this string streamId)
            => (streamId.Length > 64 || streamId.HasInvalidCharacters())
             ? throw new ArgumentException("Max length is 64 and following characters are restricted and cannot be used: '/', '\\', '?', '#'", nameof(streamId))
             : streamId;
    }
}