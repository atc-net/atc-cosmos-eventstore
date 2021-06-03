namespace Atc.Cosmos.EventStore.Streams.Validators
{
    /// <summary>
    /// Responsible for validating if the stream is required to contain events and if not then throw <see cref="StreamVersionConflictException"/>.
    /// </summary>
    public class StreamNotEmptyValidator : IStreamValidator
    {
        public void Validate(IStreamMetadata metadata, StreamVersion version)
        {
            // Pass validation if the required version is not NotEmpty.
            if (version != StreamVersion.NotEmpty)
            {
                return;
            }

            if (metadata.Version <= StreamVersion.StartOfStream)
            {
                throw new StreamVersionConflictException(
                    metadata.StreamId,
                    metadata.Version,
                    version,
                    StreamConflictReason.StreamIsEmpty,
                    $"Stream is expected to be empty but found {metadata.Version} events.");
            }
        }
    }
}