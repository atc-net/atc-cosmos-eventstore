using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BigBang.Cosmos.EventStore.Cqrs
{
    public class CommandDefinition : ICommandDefinition
    {
        private readonly IEventStream stream;
        private readonly CommandOptions options;

        public CommandDefinition(IEventStream stream, CommandOptions options)
        {
            this.stream = stream;
            this.options = options;
        }

        public async Task<ICommandContext> FromStreamAsync(
            string streamId,
            string? etag,
            string? correlationId = null,
            CancellationToken cancellationToken = default)
        {
            var models = options.Projections.ToDictionary(p => p.ModelType, p => p.CreateModel());
            await foreach (var events in stream.ReadStreamAsync(streamId, cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                foreach (var projection in options.Projections)
                {
                    projection.ApplyEvents(events, models[projection.ModelType]);
                }
            }

            return new CommandContext(stream, streamId, etag, models, options.ConflictResolution);
        }
    }
}