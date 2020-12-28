using System.Collections.Generic;

namespace BigBang.Cosmos.EventStore.Cqrs
{
    public class CommandOptions
    {
        private readonly List<ICommandProjection> projections = new List<ICommandProjection>();

        public CommandOptions(string commandName)
        {
            CommandName = commandName;
        }

        public string CommandName { get; }

        public IReadOnlyCollection<ICommandProjection> Projections => projections;

        public IConflictResolver ConflictResolution { get; set; } = new FailConflictResolver();

        public void AddProjection<TModel, TProjection>()
            where TModel : class, new()
            where TProjection : ICommandProjection<TModel>, new()
            => projections.Add(new CommandProjection<TModel, TProjection>());
    }
}