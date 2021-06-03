namespace Atc.Cosmos.EventStore
{
    public interface IEvent
    {
        object Data { get; }

        IEventProperties Properties { get; }
    }
}