using System.Collections.Generic;
using System.Data;
using Dapper.SqlExtensions.Adapters;

namespace Dapper.SqlExtensions
{
    public static class AdapterFactory
    {
        private static readonly Dictionary<string, ISqlAdapter> AdapterDictionary = new Dictionary<string, ISqlAdapter>(6)
        {
            ["sqlconnection"] = new SqlServerAdapter(),
            ["sqlceconnection"] = new GenericSqlAdapter(),
            ["npgsqlconnection"] = new PostgresAdapter(),
            ["sqliteconnection"] = new GenericSqlAdapter(),
            ["mysqlconnection"] = new GenericSqlAdapter(),
            ["fbconnection"] = new GenericSqlAdapter()
        };

        public static void Register(string name, ISqlAdapter adapter)
        {
            AdapterDictionary[name] = adapter;
        }
        
        public static ISqlAdapter GetAdapter(IDbConnection connection)
        {
            var adapterKey = connection.GetType().Name.ToLower();
            return AdapterDictionary.TryGetValue(adapterKey, out var adapter) ? adapter : new GenericSqlAdapter();
        }
    }
}