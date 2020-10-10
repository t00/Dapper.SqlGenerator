using System.Text;

namespace Dapper.SqlGenerator.Adapters
{
    public class SqlServerAdapter : BaseSqlAdapter, ISqlAdapter
    {
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

        public string Insert<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, bool insertKeys)
        {
            var sb = new StringBuilder();
            AddInsert(sb, modelBuilder, table, insertKeys);
            return sb.ToString();
        }

        public string InsertReturn<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, bool insertKeys)
        {
            var sb = new StringBuilder();
            AddInsert(sb, modelBuilder, table, insertKeys);
            sb.Append(" RETURNING ");
            sb.Append(modelBuilder.GetColumns<TEntity>(ColumnSelection.Keys));
            return sb.ToString();
        }
    }
}