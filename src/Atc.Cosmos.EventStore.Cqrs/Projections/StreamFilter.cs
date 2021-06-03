using System;
using System.Collections.Generic;
using System.Linq;

namespace Atc.Cosmos.EventStore.Cqrs.Projections
{
    public class StreamFilter
    {
        private readonly IReadOnlyList<Func<string, bool>> validators;
        private readonly bool endsOnAcceptAll;

        public StreamFilter(string filter)
        {
            validators = filter
                .Split(
                    new[] { EventStreamId.PartSeperator },
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(p => (p == "*" || p == "**")
                    ? CreateEvaluateAll()
                    : CreateEvaluation(p))
                .ToArray();
            endsOnAcceptAll = filter
                .Split(
                    new[] { EventStreamId.PartSeperator },
                    StringSplitOptions.RemoveEmptyEntries)[^1] == "**";
        }

        public bool Evaluate(StreamId streamId)
        {
            var parts = EventStreamId
                .FromStreamId(streamId)
                .Parts;

            var index = 0;
            foreach (var eval in validators)
            {
                if (index >= parts.Count || !eval(parts[index++]))
                {
                    return false;
                }
            }

            return parts.Count <= validators.Count
                || endsOnAcceptAll;
        }

        private static Func<string, bool> CreateEvaluation(string expected)
            => (input) => expected == input;

        private static Func<string, bool> CreateEvaluateAll()
            => (input) => true;
    }
}