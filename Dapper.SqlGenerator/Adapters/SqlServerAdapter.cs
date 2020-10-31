using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.SqlGenerator.Adapters
{
    public class SqlServerAdapter : GenericSqlAdapter
    {
        private const int MaxParams = 2100;

        private const int MaxQuerySize = 65536;
        
        public SqlServerAdapter(INameConverter[] tableNameConverters, INameConverter[] columnNameConverters, IEnumerable<Type> nonPrimitiveTypes = null)
            : base(tableNameConverters, columnNameConverters, nonPrimitiveTypes)
        {
        }
        
        public override string EscapeColumnName(string name)
        {
            // TODO: escape quotes
            return $"[{name}]";
        }
        
        public override string EscapeTableName(string name)
        {
            // TODO: escape quotes
            return $"[{name}]";
        }

        public override string InsertReturn<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, bool insertKeys, string columnSet)
        {
            var sb = new StringBuilder();
            var selection = ColumnSelection.NonKeys | ColumnSelection.Write;
            if (insertKeys)
            {
                selection |= ColumnSelection.Keys;
            }

            sb.Append("INSERT INTO ");
            sb.Append(GetTableName(table));
            sb.Append(" (");
            sb.Append(modelBuilder.GetColumns<TEntity>(columnSet, selection));
            sb.Append(") OUTPUT ");
            var isFirst = true;
            foreach (var property in modelBuilder.GetProperties<TEntity>(ColumnSelection.Keys))
            {
                var escapedName = EscapeColumnName(property.Name);
                if (property.ComputedColumnSql != null)
                {
                    throw new SqlGenerationException("Computed columns cannot be returned from INSERT");
                }

                if (!isFirst)
                {
                    sb.Append(',');
                }
                
                if (property.ColumnName != null)
                {
                    var escapedColumnName = EscapeColumnName(property.ColumnName);
                    if (!string.Equals(property.ColumnName, property.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append($"INSERTED.{escapedColumnName} AS {escapedName}");
                    }
                    else
                    {
                        sb.Append($"INSERTED.{escapedColumnName}");
                    }
                }
                else
                {
                    sb.Append($"INSERTED.{escapedName}");
                }

                isFirst = false;
            }
            
            sb.Append(" VALUES (");
            sb.Append(modelBuilder.GetParams<TEntity>(columnSet, selection));
            sb.Append(")");

            return sb.ToString();
        }
        
        public override string Merge<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, string mergeSet, bool insertKeys, string columnSet)
        {
            // Preferred method 2017 - https://michaeljswart.com/2017/07/sql-server-upsert-patterns-and-antipatterns/
            var sb = new StringBuilder();
            var mergeSelection = ColumnSelection.Keys | ColumnSelection.NonKeys | ColumnSelection.Write;
            sb.Append("SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;BEGIN TRAN;IF EXISTS (SELECT * FROM ");
            sb.Append(GetTableName(table));
            sb.Append(" WITH (UPDLOCK) WHERE ");
            sb.Append(modelBuilder.GetColumnEqualParams<TEntity>(mergeSet, mergeSelection,null, " AND "));
            sb.Append(") ");
            AddUpdate(sb, modelBuilder, table, columnSet);
            sb.Append("; ELSE ");
            AddInsert(sb, modelBuilder, table, insertKeys, columnSet);
            sb.Append("; COMMIT");
            return sb.ToString();
       }
    }
}