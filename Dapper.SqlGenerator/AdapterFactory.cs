using System;
using System.Collections.Generic;
using System.Data;
using Dapper.SqlGenerator.Adapters;

namespace Dapper.SqlGenerator
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

        public static Func<IDbConnection, ISqlAdapter> AdapterLookup { get; set; }

        public static void Register(Type connectionType, ISqlAdapter adapter)
        {
            var adapterKey = connectionType.Name.ToLower();
            AdapterDictionary[adapterKey] = adapter;
        }
        
        public static ISqlAdapter GetAdapter(IDbConnection connection)
        {
            if (AdapterLookup != null)
            {
                return AdapterLookup(connection);
            }
            
            var adapterKey = connection.GetType().Name.ToLower();
            return AdapterDictionary.TryGetValue(adapterKey, out var adapter) ? adapter : new GenericSqlAdapter();
        }
    }
}