using Atc.Cosmos.EventStore.Cosmos;

namespace Atc.Cosmos.EventStore.Events;

internal static class EventDocumentExtensions
{
    public static IReadOnlyCollection<T> ThrowIfEventLimitExceeded<T>(
        this IReadOnlyCollection<T> events,
        int limit = CosmosConstants.EventLimit)
    {
        if (events.Count > limit)
        {
            throw new EventLimitExceededException(events.Count, limit);
        }

        return events;
    }
}