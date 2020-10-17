using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace BigBang.Cosmos.EventStore.Cqrs
{
    public class CommandContext : ICommandContext
    {
        private readonly IEventStream stream;
        private readonly string streamId;
        private readonly string? etag;
        private readonly IConflictResolver conflictResolver;
        private readonly List<object> events;
        private readonly ReadOnlyDictionary<Type, object> models;

        public CommandContext(IEventStream stream, string streamId, string? etag, Dictionary<Type, object> models, IConflictResolver conflictResolver)
        {
            this.stream = stream;
            this.streamId = streamId;
            this.etag = etag;
            this.conflictResolver = conflictResolver;
            this.models = new ReadOnlyDictionary<Type, object>(models);
            events = new List<object>();
        }

        public void ApplyEvent<TEvent>(TEvent @event) where TEvent : class, new()
            => events.Add(@event);

        public async Task<CommandResponse> CommitAsync(CancellationToken cancellationToken = default)
        {
            CommandResponse response;

            do
            {
                var writeResponse = await stream.WriteEventsAsync(streamId, events, etag, cancellationToken: cancellationToken);
                response = CommandResponseFactory.FromEventStreamResponse(writeResponse);
            }
            while (conflictResolver.RetryRequired(response, etag));

            return response;
        }

        public TModel GetProjectionModel<TModel>()
            where TModel : class, new()
            => (TModel)models[typeof(TModel)];
    }
}