using System;
using System.Collections.Generic;
using System.Linq;
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
            sb.Append(" RETURNING ");
            sb.Append(modelBuilder.GetColumns<TEntity>(ColumnSelection.Keys));
            return sb.ToString();
        }

        public virtual string Update<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, string columnSet)
        {
            var sb = new StringBuilder();
            AddUpdate(sb, modelBuilder, table);
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
            sb.Append(modelBuilder.GetColumns<TEntity>(mergeSelection, mergeSet));
            sb.Append(") DO ");
            sb.Append("UPDATE ");
            sb.Append(GetTableName(table));
            sb.Append(" SET ");
            sb.Append(modelBuilder.GetColumnEqualParams<TEntity>(ColumnSelection.NonKeys | ColumnSelection.Write, columnSet));
            sb.Append(" WHERE ");
            sb.Append(modelBuilder.GetColumnEqualParams<TEntity>(mergeSelection, mergeSet, " AND "));
            return sb.ToString();
        }
    }
}