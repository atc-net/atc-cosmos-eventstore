using Atc.Cosmos.EventStore.Streams.Validators;

namespace Atc.Cosmos.EventStore.Streams;

internal class StreamWriteValidator : IStreamWriteValidator
{
    private static readonly IStreamValidator[] Validators = new IStreamValidator[]
    {
        new StreamClosedValidator(),
        new StreamEmptyValidator(),
        new StreamNotEmptyValidator(),
        new StreamExpectedVersionValidator(),
    };

    public void Validate(IStreamMetadata metadata, StreamVersion version)
        => Array.ForEach(
            Validators,
            v => v.Validate(metadata, version));
}
