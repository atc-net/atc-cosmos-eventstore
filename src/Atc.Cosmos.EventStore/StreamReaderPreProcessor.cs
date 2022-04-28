namespace Atc.Cosmos.EventStore
{
    /// <summary>
    ///   Called when the <see cref="IStreamMetadata"/> is first read to determine
    ///   if further reading should continue and from what position in the stream.
    /// </summary>
    /// <param name="metadata">Stream meta data.</param>
    /// <returns>
    ///   To skip reading the stream set SkipReading to <c>True</c> otherwise <c>False</c>.
    ///   
    /// </returns>
    public delegate (bool SkipReading, StreamVersion FromVersion) StreamReaderPreProcessor(IStreamMetadata metadata);
}