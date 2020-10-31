using System;
using System.Collections.Generic;
using System.Text;

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
        
        public override string InsertReturn<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, bool insertKeys, string columnSet)
        {
            var sb = new StringBuilder();
            AddInsert(sb, modelBuilder, table, insertKeys, columnSet);
            sb.Append("; SELECT last_insert_rowid() AS ");

            var keyColumns = modelBuilder.GetProperties<TEntity>(ColumnSelection.Keys);
            if (keyColumns.Count != 1)
            {
                throw new InvalidOperationException("Only one key column supported");
            }

            sb.Append(modelBuilder.Adapter.EscapeColumnName(keyColumns[0].Name));
            return sb.ToString();
        }
    }
}