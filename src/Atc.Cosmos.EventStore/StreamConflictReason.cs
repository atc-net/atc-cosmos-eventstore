namespace Atc.Cosmos.EventStore
{
    public enum StreamConflictReason
    {
        /// <summary>
        /// Stream is expected to be empty but one or more events ware found.
        /// </summary>
        StreamIsNotEmpty,

        /// <summary>
        /// Stream is expected to contain events but an empty stream is found.
        /// </summary>
        StreamIsEmpty,

        /// <summary>
        /// Stream is not at the expected version.
        /// </summary>
        ExpectedVersion,
    }
}