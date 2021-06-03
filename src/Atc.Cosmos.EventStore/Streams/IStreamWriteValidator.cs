namespace Atc.Cosmos.EventStore.Streams
{
    public interface IStreamWriteValidator
    {
        void Validate(IStreamMetadata metadata, StreamVersion version);
    }
}