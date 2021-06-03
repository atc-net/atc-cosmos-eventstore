namespace Atc.Cosmos.EventStore.Cqrs
{
    public interface ICommandContext
    {
        void AddEvent(object evt);

        object? ResponseObject { get; set; }
    }
}