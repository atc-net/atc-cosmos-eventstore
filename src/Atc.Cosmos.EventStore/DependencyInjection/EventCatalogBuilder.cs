namespace Atc.Cosmos.EventStore.DependencyInjection;

internal class EventCatalogBuilder : IEventCatalogBuilder
{
    private readonly Dictionary<EventName, Type> mappings = new();

    public IEventCatalogBuilder FromType(
        string name,
        Type type)
    {
        mappings.Add(name, type);

        return this;
    }

    public IEventCatalog Build()
        => new EventCatalog(mappings);
}