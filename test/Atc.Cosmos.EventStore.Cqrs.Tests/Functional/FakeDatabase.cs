namespace Atc.Cosmos.EventStore.Cqrs.Tests.Functional;

internal sealed class FakeDatabase
{
    private readonly Dictionary<string, object> storage = new(StringComparer.Ordinal);

    public void Save(
        string key,
        object value)
    {
        storage[key] = value;
    }

    public object? Load(string key)
    {
        storage.TryGetValue(key, out var value);
        return value;
    }
}