using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Azure.Cosmos;

namespace Atc.Cosmos.EventStore.Cosmos
{
    public static class CosmosStreamQueryBuilder
    {
        internal static QueryDefinition GetQueryDefinition(
            StreamId streamId,
            StreamVersion fromVersion,
            StreamReadFilter? filter)
        {
            var parameters = new Dictionary<string, object>();
            var query = new StringBuilder();
            query.Append("SELECT * FROM e WHERE e.pk = @partitionKey ");
            parameters["@partitionKey"] = streamId.Value;

            if (fromVersion != StreamVersion.Any && fromVersion != StreamVersion.NotEmpty)
            {
                query.Append("AND e.properties.version >= @fromVersion ");
                parameters["@fromVersion"] = fromVersion.Value;
            }

            if (filter?.IncludeEvents is not null && filter.IncludeEvents.Any())
            {
                var index = 1;
                query.Append("AND (");
                query.AppendJoin(" OR ", filter.IncludeEvents.Select(name => GetFilterExpression(name, $"@name{index++}", parameters)));
                query.Append(") ");
            }

            query.Append("ORDER BY e.properties.version");

            var definition = new QueryDefinition(query.ToString());
            foreach (var parameter in parameters)
            {
                definition.WithParameter(parameter.Key, parameter.Value);
            }

            return definition;
        }

        private static string GetFilterExpression(
            EventName name,
            string parameterName,
            Dictionary<string, object> parameters)
        {
            parameters[parameterName] = name;
            return $"e.properties.name = {parameterName}";
        }
    }
}