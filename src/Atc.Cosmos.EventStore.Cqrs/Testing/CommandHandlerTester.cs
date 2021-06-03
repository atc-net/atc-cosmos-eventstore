using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Atc.Cosmos.EventStore.Cqrs.Commands;

namespace Atc.Cosmos.EventStore.Cqrs.Testing
{
    public class CommandHandlerTester<TCommand> :
        ICommandGiven<TCommand>,
        ICommandWhen<TCommand>,
        ICommandThen
        where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> handler;
        private TCommand? command;

        public CommandHandlerTester(ICommandHandler<TCommand> handler)
        {
            this.handler = handler;
        }

        public ICommandWhen<TCommand> GivenEvents(params object[] events)
        {
            var ver = 1;
            foreach (var evt in events)
            {
                ApplyEvent(evt, CreateMetadata(ver++));
            }

            return this;
        }

        public ICommandThen WhenExecuting(TCommand command)
        {
            this.command = command;

            return this;
        }

        public async Task ThenExpectEvents(
            Action<CommandContext> assert,
            CancellationToken cancellationToken = default)
            => assert.Invoke(
                await ExecuteAsync(
                    cancellationToken)
                .ConfigureAwait(false));

        public async Task ThenExpectException<TException>(
            CancellationToken cancellationToken = default)
            where TException : Exception
        {
            try
            {
                await ExecuteAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (TException)
            {
                return;
            }

            throw new InvalidOperationException($"{typeof(TException).Name} not thrown");
        }

        public async Task ThenExpectException<TException>(
            Predicate<TException> predicate,
            CancellationToken cancellationToken = default)
            where TException : Exception
        {
            try
            {
                await ExecuteAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (TException ex)
             when (predicate(ex))
            {
                return;
            }

            throw new InvalidOperationException($"{typeof(TException).Name} not thrown");
        }

        private static EventMetadata CreateMetadata(long version)
            => new(
                Guid.NewGuid().ToString(),
                new EventStreamId("test"),
                DateTimeOffset.UtcNow,
                version,
                CorrelationId: null,
                CausationId: null);

        private static void ProjectTypedEvent<TEvent>(object projection, TEvent evt, EventMetadata metadata)
            where TEvent : class
            => (projection as IConsumeEvent<TEvent>)?.Consume(evt, metadata);

        private async Task<CommandContext> ExecuteAsync(CancellationToken cancellationToken)
        {
            var context = new CommandContext();

            await handler
                .ExecuteAsync(command!, context, cancellationToken)
                .ConfigureAwait(false);

            return context;
        }

        private void ApplyEvent(object evt, EventMetadata metadata)
        {
            var action = GetType()
                .GetRuntimeMethods()
                .First(m => m.Name == nameof(ProjectTypedEvent))
                .MakeGenericMethod(evt.GetType());

            // Project event as a strongly type event on projection.
            action.Invoke(null, new object[] { handler, evt, metadata });
        }
    }
}