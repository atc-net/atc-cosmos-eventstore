namespace Atc.Cosmos.EventStore.Streams.Validators;

/// <summary>
/// Responsible for validating if a stream is at an expected version and throw if not.
/// </summary>
internal class StreamExpectedVersionValidator : IStreamValidator
{
    public void Validate(IStreamMetadata metadata, StreamVersion version)
    {
        if (version == StreamVersion.Any || version == StreamVersion.NotEmpty)
        {
            return;
        }

        // If the stream version is not equal to the required version required, then throw.
        if (metadata.Version != version)
        {
            throw new StreamVersionConflictException(
                metadata.StreamId,
                metadata.Version,
                version,
                StreamConflictReason.ExpectedVersion,
                $"Stream version was expected to be {version.Value} but is at {metadata.Version.Value}.");
        }
    }
}