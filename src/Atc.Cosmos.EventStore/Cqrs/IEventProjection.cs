namespace BigBang.Cosmos.EventStore.Cqrs
{
    public interface IEventProjection<in TModel, in TEvent>
        where TModel : class, new()
        where TEvent : class
    {
        void ApplyEvent(TModel model, TEvent @event, EventProperties properties);
    }
}