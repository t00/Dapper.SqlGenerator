using System;
using System.Collections.Generic;

namespace Dapper.SqlGenerator.Adapters
{
    public class PostgresAdapter : GenericSqlAdapter
    {
        private const int MaxQuerySize = int.MaxValue;
        
        public PostgresAdapter(INameConverter[] tableNameConverters, INameConverter[] columnNameConverters, IEnumerable<Type> nonPrimitiveTypes = null)
            : base(tableNameConverters, columnNameConverters, nonPrimitiveTypes)
        {
        }
    }
}