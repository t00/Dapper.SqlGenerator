using System.Collections.Concurrent;
using System.Data;

namespace Dapper.SqlGenerator
{
    public static class DapperSqlGenerator
    {
        private static readonly ConcurrentDictionary<string, ModelBuilder> Options = new ConcurrentDictionary<string, ModelBuilder>();
        
        public static ModelBuilder Configure(string connectionString = null)
        {
            return Options.GetOrAdd(connectionString ?? string.Empty, _ => new ModelBuilder());
        }
        
        public static ModelBuilder SqlBuilder(this IDbConnection connection)
        {
            return EnsureAdapter(connection);
        }

        private static ModelBuilder EnsureAdapter(IDbConnection connection)
        {
            var builder = Options.GetOrAdd(connection.ConnectionString, _ => Options.TryGetValue(string.Empty, out var defaultBuilder) ? defaultBuilder : new ModelBuilder());
            if (builder.Adapter == null)
            {
                builder.Adapter = AdapterFactory.GetAdapter(connection);
            }

            return builder;
        }
    }
}
