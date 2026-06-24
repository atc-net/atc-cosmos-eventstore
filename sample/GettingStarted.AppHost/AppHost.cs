var builder = DistributedApplication.CreateBuilder(args);

// This sample connects to the locally-installed Azure Cosmos DB Emulator at https://localhost:8081
// (both projects fall back to that endpoint via AddSampleEventStore). Start the emulator before
// launching the AppHost. The Linux preview emulator container is intentionally not used here:
// its transactional-batch responses omit the written content, which breaks event writes.

// Web API is the primary sample. Scalar (at /scalar/v1) is surfaced on the dashboard
// for trying the endpoints interactively.
builder.AddProject<Projects.Api>("webapi")
    .WithScalarUrl();

// Console worker is started on demand from the dashboard ("Start" button), showing how the
// same domain types drive a hosted background worker.
builder.AddProject<Projects.Console>("console")
    .WithExplicitStart();

await builder.Build().RunAsync();