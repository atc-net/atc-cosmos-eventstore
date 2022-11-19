namespace Atc.Cosmos.EventStore.Cqrs;

public static class CommandResultExtensions
{
    public static T GetResponse<T>(this CommandResult result)
        => (T)result.Response!;
}