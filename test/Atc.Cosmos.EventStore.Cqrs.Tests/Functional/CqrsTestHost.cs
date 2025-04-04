using Atc.Cosmos.EventStore.Streams;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Atc.Cosmos.EventStore.Cqrs.Tests.Functional;

#nullable enable

public class CqrsTestHost : IAsyncDisposable
{
    private readonly WebApplication host;

    public CqrsTestHost()
    {
        host = CreateHostBuilder().Build();
    }

    public IServiceProvider Services => host.Services;

    public async Task StartAsync()
    {
        await host.StartAsync();
    }

    public async Task StopAsync()
    {
        await host.StopAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await host.StopAsync();
        await host.DisposeAsync();
    }

    private static WebApplicationBuilder CreateHostBuilder()
    {
        // Build a host with Atc EventStore
        var webApplicationBuilder = WebApplication.CreateEmptyBuilder(new WebApplicationOptions()
        {
            ApplicationName = "Atc.Cosmos.EventStore.Cqrs.Tests",
        });

        webApplicationBuilder.WebHost.UseTestServer();

        // Configure EventStore
        webApplicationBuilder.Services.AddEventStore(eventStoreBuilder =>
        {
            eventStoreBuilder.UseEvents(c => c.FromAssembly<CqrsTestHost>());
            eventStoreBuilder.UseCQRS(c =>
            {
                c.AddCommandsFromAssembly<CqrsTestHost>();
                c.AddProjectionJob<TimeProjection>("TimeProjection");
            });
        });

        // Use InMemoryEventStoreClient which actually works
        webApplicationBuilder.Services.Replace(
            ServiceDescriptor.Singleton<IEventStoreClient, InMemoryEventStoreClient>());

        // Remove unused registrations
        webApplicationBuilder.Services.RemoveAll<IStreamInfoReader>();
        webApplicationBuilder.Services.RemoveAll<IStreamMetadataReader>();
        webApplicationBuilder.Services.RemoveAll<IStreamReader>();
        webApplicationBuilder.Services.RemoveAll<IStreamWriter>();

        webApplicationBuilder.Services.AddSingleton<FakeDatabase>();

        return webApplicationBuilder;
    }
}