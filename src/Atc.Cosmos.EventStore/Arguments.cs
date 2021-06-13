using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Atc.Cosmos.EventStore
{
    [DebuggerStepThrough]
    internal static class Arguments
    {
        internal static IReadOnlyCollection<object> EnsureNoNullValues(IReadOnlyCollection<object> events, string argumentName)
        {
            if (events is null)
            {
                throw new ArgumentNullException(argumentName);
            }

            if (events.Any(e => e is null))
            {
                throw new ArgumentException("Null values not allowed", argumentName);
            }

            return events;
        }

        internal static object EnsureNotNull(object argumentValue, string argumentName)
        {
            if (argumentValue is null)
            {
                throw new ArgumentNullException(argumentName);
            }

            return argumentValue;
        }

        internal static T EnsureNotNull<T>(T? argumentValue, string argumentName)
        {
            if (argumentValue is null)
            {
                throw new ArgumentNullException(argumentName);
            }

            return argumentValue;
        }

        internal static StreamVersion EnsureValueRange(StreamVersion streamVersion, string argumentName)
        {
            if (streamVersion < StreamVersion.NotEmpty)
            {
                throw new ArgumentOutOfRangeException(
                    argumentName,
                    $"Stream version {streamVersion.Value} is outside of valid range [{StreamVersion.NotEmptyValue}-{StreamVersion.EndOfStreamValue}].");
            }

            return streamVersion;
        }
    }
}