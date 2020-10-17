using System;
using System.Text;

namespace BigBang.Cosmos.EventStore
{
    /// <summary>
    /// Extension methods for working with etag's.
    /// </summary>
    public static class ETagExtensions
    {
        /// <summary>
        /// Generates and sets the etag of an event properties.
        /// </summary>
        /// <param name="properties">Event property to set the etag on.</param>
        public static void ApplyETag(this EventProperties properties)
            => properties.Etag = Convert.ToBase64String(
                Encoding.ASCII.GetBytes(
                    $"{properties.StreamName}/{properties.StreamId}/{properties.Version}"));

        /// <summary>
        /// Get version from an etag.
        /// </summary>
        /// <exception cref="ArgumentException">Throws when etag is malformed.</exception>
        /// <param name="etag">Etag value.</param>
        /// <param name="streamId">Id of the stream, this is to validate the etag belongs the correct stream id.</param>
        /// <returns>Stream version or <seealso cref="ExpectedVersion.Any"/> when <paramref name="etag"/> is null.</returns>
        public static long GetVersionFromEtag(this string? etag, string streamId)
        {
            if (string.IsNullOrEmpty(etag))
            {
                return ExpectedVersion.Any;
            }

            var etagString = Encoding.ASCII.GetString(
                Convert.FromBase64String(etag));

            var parts = etagString.Split("/");
            if (parts.Length != 3)
            {
                throw new ArgumentException("Invalid etag", nameof(etag));
            }

            if (!parts[1].Equals(streamId, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException("Invalid etag", nameof(etag));
            }

            if (!long.TryParse(parts[1], out var version))
            {
                throw new ArgumentException("Invalid etag", nameof(etag));
            }

            return version;
        }
    }
}