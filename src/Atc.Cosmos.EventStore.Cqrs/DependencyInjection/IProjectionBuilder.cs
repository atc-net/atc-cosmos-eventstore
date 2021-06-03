using Atc.Cosmos.EventStore;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface IProjectionBuilder
    {
        /// <summary>
        /// Filter on stream id for events projected.
        /// </summary>
        /// <param name="filter">Filter pattern.</param>
        /// <returns>The builder.</returns>
        IProjectionBuilder WithFilter(string filter);

        IProjectionBuilder WithJobName(string name);

        IProjectionBuilder WithExceptionHandler(ProcessExceptionHandler handler);
    }
}