namespace Atc.Cosmos.EventStore.Streams.Validators;

/// <summary>
/// Responsible for validating if the stream is required to be empty and if not then throw <see cref="StreamVersionConflictException"/>.
/// </summary>
internal class StreamEmptyValidator : IStreamValidator
{
    public void Validate(IStreamMetadata metadata, StreamVersion version)
    {
        // Pass validation if the required version is not start of stream.
        if (version != StreamVersion.StartOfStream)
        {
            return;
        }

        if (metadata.Version != StreamVersion.StartOfStream)
        {
            throw new StreamVersionConflictException(
                metadata.StreamId,
                metadata.Version,
                version,
                StreamConflictReason.StreamIsNotEmpty,
                "Stream is expected to be empty.");
        }
    }
}