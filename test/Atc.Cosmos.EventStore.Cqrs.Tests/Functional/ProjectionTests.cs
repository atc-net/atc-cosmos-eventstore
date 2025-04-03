using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Atc.Cosmos.EventStore.Cqrs.Tests.Functional;

[Trait("Category", "Functional")]
public class ProjectionTests : IAsyncLifetime
{
    private CqrsTestHost host = null!;

    [Fact]
    public async Task Projection_is_triggered_when_command_is_executed()
    {
        var tick = DateTime.UtcNow.Ticks;
        _ = await host.Services.GetRequiredService<ICommandProcessorFactory>()
            .Create<MakeTimeTickCommand>()
            .ExecuteAsync(new MakeTimeTickCommand(tick), CancellationToken.None);

        // Assert that projection was triggered and saved our tick to database
        var database = host.Services.GetRequiredService<FakeDatabase>();
        var storedTick = database.Load("TimeProjection");
        Assert.Equal(tick, storedTick);
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