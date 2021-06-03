namespace Atc.Cosmos.EventStore.Streams.Validators
{
    public class StreamClosedValidator : IStreamValidator
    {
        public void Validate(IStreamMetadata metadata, StreamVersion version)
        {
            if (metadata.State == StreamState.Closed)
            {
                throw new StreamClosedException(metadata.StreamId);
            }
        }
    }
}