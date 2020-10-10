using System.Text;

namespace Dapper.SqlGenerator.Adapters
{
    public abstract class BaseSqlAdapter : IBaseSqlAdapter
    {
        public abstract string EscapeColumnName(string name);

        public abstract string EscapeTableName(string name);

        public virtual bool IsSelected(PropertyBuilder property, ColumnSelection selection)
        {
            return !property.Ignored
                   && ((property.IsKey && selection.HasFlag(ColumnSelection.Keys)) || (!property.IsKey && selection.HasFlag(ColumnSelection.NonKeys)))
                   && (property.ComputedColumnSql == null || selection.HasFlag(ColumnSelection.Computed));
        }

        public virtual string GetColumn(PropertyBuilder property, ColumnSelection selection)
        {
            var escapedName = EscapeColumnName(property.Name);
            if (property.ComputedColumnSql != null)
            {
                return $"{property.ComputedColumnSql} AS {escapedName}";
            }
            
            if (property.ColumnName != null && property.ColumnName != property.Name)
            {
                var escapedColumnName = EscapeColumnName(property.ColumnName);
                return selection.HasFlag(ColumnSelection.Write) ? escapedColumnName : $"{escapedColumnName} AS {escapedName}";
            }

            return escapedName;
        }

        public virtual string GetParam(PropertyBuilder property, ColumnSelection selection)
        {
            if (property.ColumnType == null || !selection.HasFlag(ColumnSelection.Write))
            {
                return $"@{property.Name}";
            }
            else
            {
                return $"CAST(@{property.Name} AS {property.ColumnType})";
            }
        }

        public virtual string GetColumnEqualParam(PropertyBuilder property, ColumnSelection selection)
        {
            var escapedName = EscapeColumnName(property.ColumnName ?? property.Name);
            if (property.ColumnType != null)
            {
                return $"{escapedName}=CAST(@{property.Name} AS {property.ColumnType})";
            }
            else
            {
                return $"{escapedName}=@{property.Name}";
            }
        }
        
        protected void AddInsert<TEntity>(StringBuilder sb, ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, bool insertKeys)
        {
            var selection = ColumnSelection.NonKeys | ColumnSelection.Write;
            if (insertKeys)
            {
                selection |= ColumnSelection.Keys;
            }

            sb.Append("INSERT INTO ");
            sb.Append(modelBuilder.Adapter.EscapeTableName(table.TableName));
            sb.Append(" (");
            sb.Append(modelBuilder.GetColumns<TEntity>(selection));
            sb.Append(") VALUES (");
            sb.Append(modelBuilder.GetParams<TEntity>(selection));
            sb.Append(")");
        }
    }
}