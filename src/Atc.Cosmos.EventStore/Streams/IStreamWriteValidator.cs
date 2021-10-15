namespace Atc.Cosmos.EventStore.Streams
{
    internal interface IStreamWriteValidator
    {
        void Validate(IStreamMetadata metadata, StreamVersion version);
    }
}