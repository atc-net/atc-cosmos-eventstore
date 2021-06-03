namespace Atc.Cosmos.EventStore.Streams
{
    public interface IStreamReadValidator
    {
        void Validate(IStreamMetadata metadata, StreamVersion version);
    }
}