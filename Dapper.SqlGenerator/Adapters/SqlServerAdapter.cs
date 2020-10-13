using System.Linq;
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
            // Preferred method 2017 - https://michaeljswart.com/2017/07/sql-server-upsert-patterns-and-antipatterns/
            var sb = new StringBuilder();
            var mergeSelection = ColumnSelection.Keys | ColumnSelection.NonKeys | ColumnSelection.Write;
            sb.Append("SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;BEGIN TRAN;IF EXISTS (SELECT * FROM ");
            sb.Append(GetTableName(table));
            sb.Append(" WITH (UPDLOCK) WHERE ");
            var condition = string.Join(" AND ", modelBuilder
                .GetProperties<TEntity>(mergeSelection, mergeSet)
                .Select(x => modelBuilder.Adapter.GetColumnEqualParam(x, mergeSelection)).Where(x => x != null));
            sb.Append(condition);
            sb.Append(") ");
            AddUpdate(sb, modelBuilder, table, columnSet);
            sb.Append("; ELSE ");
            AddInsert(sb, modelBuilder, table, insertKeys, columnSet);
            sb.Append("; COMMIT");
            return sb.ToString();
       }
    }
}