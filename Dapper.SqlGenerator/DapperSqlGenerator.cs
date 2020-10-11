using System.Collections.Concurrent;
using System.Data;

namespace Dapper.SqlGenerator
{
    public static class DapperSqlGenerator
    {
        private static readonly ConcurrentDictionary<string, ModelBuilder> ModelBuilders = new ConcurrentDictionary<string, ModelBuilder>();
        
        public static ModelBuilder Configure(string connectionString = null)
        {
            return ModelBuilders.GetOrAdd(connectionString ?? string.Empty, _ => new ModelBuilder());
        }
        
        public static ISql Sql(this IDbConnection connection)
        {
            return EnsureAdapter(connection);
        }

        private static ModelBuilder EnsureAdapter(IDbConnection connection)
        {
            var builder = ModelBuilders.GetOrAdd(connection.ConnectionString, _ => ModelBuilders.TryGetValue(string.Empty, out var defaultBuilder) ? new ModelBuilder(defaultBuilder) : new ModelBuilder());
            if (builder.Adapter == null)
            {
                builder.Adapter = AdapterFactory.GetAdapter(connection);
            }

            return builder;
        }
    }
}
