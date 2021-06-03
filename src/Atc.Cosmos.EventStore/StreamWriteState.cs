namespace Atc.Cosmos.EventStore
{
    public enum StreamWriteState
    {
        /// <summary>
        /// Always write to stream.
        /// </summary>
        Any,

        /// <summary>
        /// Only write to stream if it is empty (not create).
        /// </summary>
        Empty,

        /// <summary>
        /// Only write to stream if it already contains events.
        /// </summary>
        Exists,
    }
}