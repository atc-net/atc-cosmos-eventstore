using Atc.Cosmos.EventStore.Cqrs;
using Microsoft.Extensions.Hosting;

namespace GettingStarted;

public class ConsoleHostedService(ICommandProcessorFactory commandProcessorFactory) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid().ToString("N");

        await commandProcessorFactory
            .Create<CreateCommand>()
            .ExecuteAsync(
                new CreateCommand(id, "First"),
                cancellationToken);

        await commandProcessorFactory
            .Create<UpdateNameCommand>()
            .ExecuteAsync(
                new UpdateNameCommand(id, "Second"),
                cancellationToken);

        await commandProcessorFactory
            .Create<UpdateNameCommand>()
            .ExecuteAsync(
                new UpdateNameCommand(id, "Second"),
                cancellationToken);

        await commandProcessorFactory
            .Create<UpdateNameCommand>()
            .ExecuteAsync(
                new UpdateNameCommand(id, "Second"),
                cancellationToken);

        await commandProcessorFactory
            .Create<DeleteCommand>()
            .ExecuteAsync(
                new DeleteCommand(id, "Deleted"),
                cancellationToken);

        await commandProcessorFactory
            .Create<DeleteCommand>()
            .ExecuteAsync(
                new DeleteCommand(id, "Deleted"),
                cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}