using Atc.Cosmos;
using Atc.Cosmos.EventStore.Cqrs;

namespace GettingStarted.Storage;

[ProjectionFilter(SampleEventStreamId.FilterIncludeAllEvents)]
public class SampleProjection(
    ICosmosReader<SampleReadModel> reader,
    ICosmosWriter<SampleReadModel> writer) :
    IProjection,
    IConsumeEvent<AddedEvent>,
    IConsumeEvent<NameChangedEvent>,
    IConsumeEvent<AddressChangedEvent>,
    IConsumeEvent<DeletedEvent>
{
    private SampleReadModel view = null!;
    private bool deleted = false;

    public Task<ProjectionAction> FailedAsync(
        Exception exception,
        CancellationToken cancellationToken) =>
        Task.FromResult(ProjectionAction.Continue);

    public async Task InitializeAsync(
        EventStreamId id,
        CancellationToken cancellationToken)
    {
        var streamId = new SampleEventStreamId(id);
        view = await reader.FindAsync(
                   streamId.Id,
                   streamId.Id,
                   cancellationToken) ??
               new SampleReadModel
               {
                   Id = streamId.Id,
               };
    }

    public Task CompleteAsync(
        CancellationToken cancellationToken) =>
        deleted
            ? writer.TryDeleteAsync(view!.Id, view!.PartitionKey, cancellationToken)
            : writer.WriteAsync(view, cancellationToken);

    public void Consume(AddedEvent evt, EventMetadata metadata)
    {
        view.Name = evt.Name;
        view.Address = evt.Address;
        deleted = false;
    }

    public void Consume(NameChangedEvent evt, EventMetadata metadata)
    {
        view.Name = evt.NewName;
    }

    public void Consume(AddressChangedEvent evt, EventMetadata metadata)
    {
        view.Address = evt.NewAddress;
    }

    public void Consume(DeletedEvent evt, EventMetadata metadata)
    {
        deleted = true;
    }
}