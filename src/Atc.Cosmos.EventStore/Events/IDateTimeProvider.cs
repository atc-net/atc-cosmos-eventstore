using System;

namespace Atc.Cosmos.EventStore.Events
{
    public interface IDateTimeProvider
    {
        DateTimeOffset GetDateTime();
    }
}