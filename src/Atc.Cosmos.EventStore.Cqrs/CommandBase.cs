using System;
using System.Diagnostics.CodeAnalysis;

namespace Atc.Cosmos.EventStore.Cqrs
{
    public abstract record CommandBase<TStreamId>(
        TStreamId StreamId,
        string? CommandId = default,
        string? CorrelationId = default,
        EventStreamVersion? RequiredVersion = default,
        OnConflict Behavior = OnConflict.Fail,
        int BehaviorCount = 3) : ICommand
        where TStreamId : EventStreamId
    {
        public string CommandId { get; init; } = CommandId ?? Guid.NewGuid().ToString();

        [SuppressMessage(
            "Design",
            "CA1033:Interface methods should be callable by child types",
            Justification = "By Design")]
        EventStreamId ICommand.GetEventStreamId() => StreamId;
    }
}