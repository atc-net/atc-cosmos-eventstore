await Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        services.AddSampleEventStore(context.Configuration);
        services.AddHostedService<ConsoleHostedService>();
    })
    .RunConsoleAsync();