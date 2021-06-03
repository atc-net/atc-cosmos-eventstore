namespace Atc.Cosmos.EventStore.Cqrs
{
    /// <summary>
    /// Defines the behavior when writing to a stream results in a conflict.
    /// </summary>
    public enum OnConflict
    {
        /// <summary>
        /// Fail command on stream write conflict.
        /// </summary>
        Fail,

        /// <summary>
        /// Retry writing events on stream write conflict.
        /// </summary>
        Retry,

        /// <summary>
        /// Rerun command on stream write conflict.
        /// </summary>
        RerunCommand,
    }
}