using System;
using System.Collections.Generic;

namespace BigBang.Cosmos.EventStore.Cqrs
{
    public class CommandProjection<TModel, TProjection> : ICommandProjection<TModel>, ICommandProjection
        where TModel : class, new()
        where TProjection : ICommandProjection<TModel>, new()
    {
        private readonly TProjection projection;

        public CommandProjection()
        {
            ModelType = typeof(TModel);
            projection = new TProjection();
        }

        public Type ModelType { get; }

        public object CreateModel() => new TModel();

        public void ApplyEvents(IReadOnlyCollection<Event> events, object model)
            => events.ApplyProjection(projection, model);
    }
}