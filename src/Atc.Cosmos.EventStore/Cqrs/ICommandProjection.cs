using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BigBang.Cosmos.EventStore.Cqrs
{
    public interface ICommandProjection
    {
        Type ModelType { get; }

        object CreateModel();

        void ApplyEvents(IReadOnlyCollection<Event> events, object model);
    }

    [SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "ByDesign")]
    public interface ICommandProjection<TModel>
        where TModel : class, new()
    {
    }
}