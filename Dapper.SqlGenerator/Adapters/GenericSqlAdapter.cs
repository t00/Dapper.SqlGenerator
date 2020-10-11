using System.Text;

namespace Dapper.SqlGenerator.Adapters
{
    public class GenericSqlAdapter : BaseSqlAdapter, ISqlAdapter
    {
        public GenericSqlAdapter(INameConverter[] tableNameConverters, INameConverter[] columnNameConverters) : base(tableNameConverters, columnNameConverters)
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

        public virtual string Insert<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, bool insertKeys)
        {
            var sb = new StringBuilder();
            AddInsert(sb, modelBuilder, table, insertKeys);
            return sb.ToString();
        }
        
        public virtual string InsertReturn<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, bool insertKeys)
        {
            var sb = new StringBuilder();
            AddInsert(sb, modelBuilder, table, insertKeys);
            sb.Append(" RETURNING ");
            sb.Append(modelBuilder.GetColumns<TEntity>(ColumnSelection.Keys));
            return sb.ToString();
        }

        public virtual string Update<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table)
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
    }
}