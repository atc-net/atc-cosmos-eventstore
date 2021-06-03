namespace Atc.Cosmos.EventStore.Events
{
    public interface IEventNameProvider
    {
        string GetName(object evt);
    }
}