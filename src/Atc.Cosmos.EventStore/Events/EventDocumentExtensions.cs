using Atc.Cosmos.EventStore.Cosmos;

namespace Atc.Cosmos.EventStore.Events;

internal static class EventDocumentExtensions
{
    public static IReadOnlyCollection<T> ThrowIfEventLimitExceeded<T>(
        this IReadOnlyCollection<T> events)
    {
        if (events.Count > CosmosConstants.EventLimit)
        {
            throw new EventLimitExceededException(
                events.Count,
                CosmosConstants.EventLimit);
        }

        return events;
    }
}