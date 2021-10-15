namespace Atc.Cosmos.EventStore.Streams
{
    internal interface IStreamValidator
    {
        void Validate(IStreamMetadata metadata, StreamVersion version);
    }
}