using System.Collections.Generic;

namespace Atc.Cosmos.EventStore
{
    /// <summary>
    /// Configure stream reading options.
    /// </summary>
    public class StreamReadOptions
    {
        /// <summary>
        /// Gets or sets the required version the stream must be at.
        /// </summary>
        public StreamVersion? RequiredVersion { get; set; }

        /// <summary>
        /// Gets or sets the type of events to read from the stream.
        /// </summary>
        public IReadOnlyCollection<EventName>? IncludeEvents { get; set; }

        /// <summary>
        /// Gets or sets a preprocessor to determine if the stream reading should continue
        /// based on it's meta data.
        /// </summary>
        public StreamReaderPreProcessor? OnMetadataRead { get; set; }
    }
}