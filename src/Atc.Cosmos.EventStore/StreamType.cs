namespace BigBang.Cosmos.EventStore
{
    /// <summary>
    /// Defines the type of stream.
    /// </summary>
    public enum StreamType
    {
        /// <summary>
        /// Stream should maintain strict order through an incrementing version for each event added.
        /// </summary>
        Versioned,

        /// <summary>
        /// All events are inserted with a time-stamp.
        /// </summary>
        Timeseries,
    }
}