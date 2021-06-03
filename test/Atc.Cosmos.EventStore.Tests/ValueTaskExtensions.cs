using System;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Tests
{
    public static class ValueTaskExtensions
    {
        public static Func<T> Awaiting<T>(this Func<ValueTask<T>> action)
#pragma warning disable CA2012 // Use ValueTasks correctly
            => () => action().GetAwaiter().GetResult();
#pragma warning restore CA2012 // Use ValueTasks correctly

        public static Action Awaiting(this Func<ValueTask> action)
#pragma warning disable CA2012 // Use ValueTasks correctly
            => () => action().GetAwaiter().GetResult();
#pragma warning restore CA2012 // Use ValueTasks correctly
    }
}