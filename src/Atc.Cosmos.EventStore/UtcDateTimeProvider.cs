namespace Atc.Cosmos.EventStore;

public class UtcDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset GetDateTime()
        => DateTimeOffset.UtcNow;
}