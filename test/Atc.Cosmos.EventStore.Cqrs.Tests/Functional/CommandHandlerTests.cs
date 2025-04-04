using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Atc.Cosmos.EventStore.Cqrs.Tests.Functional;

#nullable enable

[Trait("Category", "Functional")]
public class CommandHandlerTests : IAsyncLifetime
{
    private CqrsTestHost host = null!;

    [Fact]
    public async Task Result_from_CommandHandler_must_be_returned()
    {
        var tick = DateTime.UtcNow.Ticks;
        var commandResult = await host.Services.GetRequiredService<ICommandProcessorFactory>()
            .Create<MakeTimeTickCommand>()
            .ExecuteAsync(new MakeTimeTickCommand(tick), CancellationToken.None);

        Assert.NotNull(commandResult);
        Assert.NotNull(commandResult.Response);
        Assert.Equal(tick, commandResult.Response);
    }

    [Fact]
    public async Task CommandHandler_can_consume_existing_events()
    {
        // Produce some events by making time tick
        await MakeTimeTick(host);
        await MakeTimeTick(host);
        await MakeTimeTick(host);

        // Query events
        var result = await host.Services.GetRequiredService<ICommandProcessorFactory>()
            .Create<QueryTimeTickCommand>()
            .ExecuteAsync(new QueryTimeTickCommand(), CancellationToken.None);

        var events = Assert.IsType<List<(TimeTickedEvent Evt, EventMetadata Metadata)>>(result.Response);
        Assert.Equal(3, events.Count);

        static async Task MakeTimeTick(CqrsTestHost host)
        {
            var tick = DateTime.UtcNow.Ticks;
            var commandProcessorFactory = host.Services.GetRequiredService<ICommandProcessorFactory>();
            _ = await commandProcessorFactory
                .Create<MakeTimeTickCommand>()
                .ExecuteAsync(new MakeTimeTickCommand(tick), CancellationToken.None);
        }
    }

    [Fact]
    public async Task CommandHandler_that_consumes_events_works_when_no_existing_events_are_present()
    {
        // Query events - none exists
        var result = await host.Services.GetRequiredService<ICommandProcessorFactory>()
            .Create<QueryTimeTickCommand>()
            .ExecuteAsync(new QueryTimeTickCommand(), CancellationToken.None);

        var events = Assert.IsType<List<(TimeTickedEvent Evt, EventMetadata Metadata)>>(result.Response);
        Assert.Empty(events);
    }

    [Fact]
    public async Task Command_that_uses_RequiredVersion_Exists_must_result_in_NotFound_when_no_existing_events_are_present()
    {
        // Query events - must fail as non exists
        var result = await host.Services.GetRequiredService<ICommandProcessorFactory>()
            .Create<QueryExistingTimeTickCommand>()
            .ExecuteAsync(new QueryExistingTimeTickCommand(), CancellationToken.None);

        Assert.Equal(ResultType.NotFound, result.Result);
    }

    public async Task InitializeAsync()
    {
        host = new CqrsTestHost();
        await host.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await host.DisposeAsync();
    }
}