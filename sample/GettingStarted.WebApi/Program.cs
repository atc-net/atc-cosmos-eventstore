var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSampleEventStore(builder.Configuration);
builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapPost("/customers", async (
    CreateCustomerRequest request,
    ICommandProcessorFactory factory,
    CancellationToken ct) =>
{
    var id = Guid.NewGuid().ToString("N");
    var result = await factory
        .Create<CreateCommand>()
        .ExecuteAsync(new CreateCommand(id, request.Name, request.Address), ct);

    return result.Result == ResultType.Changed
        ? Results.Created($"/customers/{id}", new { Id = id })
        : Results.Conflict(new { result.Result });
});

app.MapGet("/customers", async (
    ICosmosReader<SampleReadModel> reader,
    CancellationToken ct) =>
{
    var customers = new List<SampleReadModel>();
    await foreach (var customer in reader.CrossPartitionQueryAsync(
        new QueryDefinition("SELECT * FROM c"),
        ct))
    {
        customers.Add(customer);
    }

    return Results.Ok(customers);
});

app.MapGet("/customers/{id}", async (
    string id,
    ICosmosReader<SampleReadModel> reader,
    CancellationToken ct) =>
{
    var view = await reader.FindAsync(id, id, ct);
    return view is null ? Results.NotFound() : Results.Ok(view);
});

app.MapPut("/customers/{id}/name", async (
    string id,
    UpdateNameRequest request,
    ICommandProcessorFactory factory,
    CancellationToken ct) =>
{
    var result = await factory
        .Create<UpdateNameCommand>()
        .ExecuteAsync(new UpdateNameCommand(id, request.Name), ct);

    return Results.Ok(new { result.Result });
});

app.MapDelete("/customers/{id}", async (
    string id,
    ICommandProcessorFactory factory,
    CancellationToken ct) =>
{
    var result = await factory
        .Create<DeleteCommand>()
        .ExecuteAsync(new DeleteCommand(id, "Deleted"), ct);

    return Results.Ok(new { result.Result });
});

await app.RunAsync();