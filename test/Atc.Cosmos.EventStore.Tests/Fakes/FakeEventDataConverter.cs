namespace Atc.Cosmos.EventStore.Tests.Fakes;

public sealed class FakeEventDataConverter : IEventDataConverter
{
    public FakeEventDataConverter(string val)
        => Val = val;

    public string Val { get; }

    public object? Convert(
        IEventMetadata metadata,
        JsonElement data,
        JsonSerializerOptions options,
        Func<object?> next)
        => Val + next.Invoke();
}