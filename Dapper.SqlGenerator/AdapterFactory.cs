using System;
using System.Collections.Generic;
using System.Data;
using Dapper.SqlGenerator.Adapters;
using Dapper.SqlGenerator.NameConverters;

namespace Dapper.SqlGenerator
{
    public static class AdapterFactory
    {
        private static readonly Dictionary<string, ISqlAdapter> AdapterDictionary = new Dictionary<string, ISqlAdapter>(6)
        {
            ["sqlconnection"] = new SqlServerAdapter(new INameConverter[] { new PluralNameConverter() }, new INameConverter[0]),
            ["npgsqlconnection"] = new PostgresAdapter(new INameConverter[] { new PluralNameConverter() }, new INameConverter[0]),
            ["sqliteconnection"] = new SqliteAdapter(new INameConverter[] { new PluralNameConverter() }, new INameConverter[0])
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
            return AdapterDictionary.TryGetValue(adapterKey, out var adapter) ? adapter : new GenericSqlAdapter(new INameConverter[] { new PluralNameConverter() }, new INameConverter[0]);
        }
    }
}