using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.SqlGenerator.Adapters
{
    public class GenericSqlAdapter : BaseSqlAdapter, ISqlAdapter
    {
        public GenericSqlAdapter(INameConverter[] tableNameConverters, INameConverter[] columnNameConverters, IEnumerable<Type> nonPrimitiveTypes = null)
            : base(tableNameConverters, columnNameConverters, nonPrimitiveTypes)
        {
        }

        public override string EscapeTableName(string name)
        {
            // TODO: escape quotes, is it even needed?
            return $"\"{name}\"";
        }

        public override string EscapeColumnName(string name)
        {
            // TODO: escape quotes, is it even needed?
            return $"\"{name}\"";
        }

        public virtual string TableExists()
        {
            return "SELECT COUNT(1) FROM information_schema.tables WHERE table_name = @table";
        }

        public string Select<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, string columnSet, ColumnSelection columnSelection, string whereSet, ColumnSelection whereSelection, string alias)
        {
            var sb = new StringBuilder();
            sb.Append("SELECT ");
            sb.Append(modelBuilder.GetColumns<TEntity>(columnSet, columnSelection, alias));
            sb.Append(" FROM ");
            sb.Append(GetTableName(table));
            if (alias != null)
            {
                sb.Append(" ");
                sb.Append(alias);
            }

            if (whereSelection != ColumnSelection.None)
            {
                var where = modelBuilder.GetColumnEqualParams<TEntity>(whereSet, whereSelection, alias, " AND ");
                if (!string.IsNullOrWhiteSpace(where))
                {
                    sb.Append(" WHERE ");
                    sb.Append(modelBuilder.GetColumnEqualParams<TEntity>(whereSet, whereSelection, alias, " AND "));
                }
            }

            return sb.ToString();
        }

        public virtual string Insert<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, bool insertKeys, string columnSet)
        {
            var sb = new StringBuilder();
            AddInsert(sb, modelBuilder, table, insertKeys, columnSet);
            return sb.ToString();
        }
        
        public virtual string InsertReturn<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, bool insertKeys, string columnSet)
        {
            var sb = new StringBuilder();
            AddInsert(sb, modelBuilder, table, insertKeys, columnSet);
            var keys = modelBuilder.GetColumns<TEntity>(null, ColumnSelection.Keys);
            if (!string.IsNullOrWhiteSpace(keys))
            {
                sb.Append(" RETURNING ");
                sb.Append(modelBuilder.GetColumns<TEntity>(null, ColumnSelection.Keys));
            }

            return sb.ToString();
        }

        public virtual string Update<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, string columnSet)
        {
            var sb = new StringBuilder();
            AddUpdate(sb, modelBuilder, table, columnSet);
            return sb.ToString();
        }

        public string Delete<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table)
        {
            var sb = new StringBuilder();
            AddDelete(sb, modelBuilder, table);
            return sb.ToString();
        }
        
        public virtual string Merge<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, string mergeSet, bool insertKeys, string columnSet)
        {
            var sb = new StringBuilder();
            var mergeSelection = ColumnSelection.Keys | ColumnSelection.NonKeys | ColumnSelection.Write;
            AddInsert(sb, modelBuilder, table, insertKeys, columnSet);
            sb.Append(" ON CONFLICT(");
            sb.Append(modelBuilder.GetColumns<TEntity>(mergeSet, mergeSelection));
            sb.Append(") DO ");
            sb.Append("UPDATE SET ");
            sb.Append(modelBuilder.GetColumnEqualParams<TEntity>(columnSet, ColumnSelection.NonKeys | ColumnSelection.Write));
            sb.Append(" WHERE ");
            sb.Append(modelBuilder.GetColumnEqualParams<TEntity>(mergeSet, mergeSelection, null, " AND "));
            return sb.ToString();
        }
    }
}