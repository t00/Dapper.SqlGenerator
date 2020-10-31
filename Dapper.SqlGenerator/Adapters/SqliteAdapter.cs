using System;
using System.Collections.Generic;

namespace Dapper.SqlGenerator.Adapters
{
    public class SqliteAdapter : GenericSqlAdapter
    {
        public SqliteAdapter(INameConverter[] tableNameConverters, INameConverter[] columnNameConverters, IEnumerable<Type> nonPrimitiveTypes = null) : base(tableNameConverters, columnNameConverters, nonPrimitiveTypes)
        {
        }

        public override string TableExists()
        {
            return "SELECT count(*) FROM sqlite_master WHERE type='table' AND name=@table";
        }
    }
}