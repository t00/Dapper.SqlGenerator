using System;
using System.Text;

namespace Dapper.SqlGenerator.Adapters
{
    public class SqlServerAdapter : BaseSqlAdapter, ISqlAdapter
    {
        private const int MaxParams = 2100;

        private const int MaxQuerySize = 65536;
        
        public SqlServerAdapter(INameConverter[] tableNameConverters, INameConverter[] columnNameConverters) : base(tableNameConverters, columnNameConverters)
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

        public string Insert<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, bool insertKeys, string columnSet)
        {
            var sb = new StringBuilder();
            AddInsert(sb, modelBuilder, table, insertKeys, columnSet);
            return sb.ToString();
        }

        public string InsertReturn<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, bool insertKeys, string columnSet)
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
            sb.Append(modelBuilder.GetColumns<TEntity>(selection, columnSet));
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
                
                if (property.ColumnName != null && property.ColumnName != property.Name)
                {
                    var escapedColumnName = EscapeColumnName(property.ColumnName);
                    sb.Append($"INSERTED.{escapedColumnName} AS {escapedName}");
                }
                else
                {
                    sb.Append($"INSERTED.{escapedName}");
                }

                isFirst = false;
            }
            
            sb.Append(" VALUES (");
            sb.Append(modelBuilder.GetParams<TEntity>(selection, columnSet));
            sb.Append(")");

            return sb.ToString();
        }

        public string Update<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, string columnSet)
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
            throw new NotImplementedException();
        }
    }
}