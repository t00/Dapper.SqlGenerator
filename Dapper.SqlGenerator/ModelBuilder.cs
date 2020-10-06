using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace Dapper.SqlGenerator
{
    public class ModelBuilder
    {
        private readonly ConcurrentDictionary<Type, EntityTypeBuilder> tableDict;
        private readonly ConcurrentDictionary<(Type type, ColumnSelection selection), string> columnsCache = new ConcurrentDictionary<(Type, ColumnSelection), string>();
        private readonly ConcurrentDictionary<(Type type, ColumnSelection selection), string> paramsCache = new ConcurrentDictionary<(Type, ColumnSelection), string>();
        private readonly ConcurrentDictionary<(Type type, ColumnSelection selection), string> columnEqualParamsCache = new ConcurrentDictionary<(Type, ColumnSelection), string>();

        public ModelBuilder()
        {
            Shared = new EntityTypeBuilder();
            tableDict = new ConcurrentDictionary<Type, EntityTypeBuilder>();
        }

        public ModelBuilder(ModelBuilder source)
        {
            Shared = source.Shared;
            tableDict = source.tableDict;
        }

        public EntityTypeBuilder Shared { get; }

        public ISqlAdapter Adapter { get; set; }

        public ModelBuilder Entity<TEntity>(Action<EntityTypeBuilder<TEntity>> action = null)
        {
            var entityBuilder = EnsureEntity<TEntity>();
            action?.Invoke(entityBuilder);
            return this;
        }

        public ModelBuilder HasDefaultKeyColumn(string propertyName, Action<PropertyBuilder> options = null, Type adapter = null)
        {
            var property = Shared.Property(propertyName, adapter);
            property.IsKey = true;
            options?.Invoke(property);
            return this;
        }

        public string Insert<TEntity>(bool insertKeys = false)
        {
            var table = EnsureEntity<TEntity>();
            var selection = (insertKeys ? ColumnSelection.Keys : ColumnSelection.None) | ColumnSelection.NonKeys | ColumnSelection.Write;
            var sb = new StringBuilder();
            sb.Append("INSERT INTO ");
            sb.Append(Adapter.EscapeTableName(table.TableName));
            sb.Append(" (");
            sb.Append(GetColumns<TEntity>(selection));
            sb.Append(") VALUES (");
            sb.Append(GetParams<TEntity>(selection));
            sb.Append(")");
            return sb.ToString();
        }
        
        public string GetColumns<TEntity>(ColumnSelection selection = ColumnSelection.Select)
        {
            return columnsCache.GetOrAdd((typeof(TEntity), selection), key => CreateColumns<TEntity>(key.selection));
        }

        public string GetParams<TEntity>(ColumnSelection selection = ColumnSelection.Select)
        {
            return paramsCache.GetOrAdd((typeof(TEntity), selection), key => CreateParams<TEntity>(key.selection));
        }

        public string GetColumnEqualParams<TEntity>(ColumnSelection selection = ColumnSelection.Select)
        {
            return columnEqualParamsCache.GetOrAdd((typeof(TEntity), selection), key => CreateColumnEqualParams<TEntity>(key.selection));
        }

        private EntityTypeBuilder<TEntity> EnsureEntity<TEntity>()
        {
            return (EntityTypeBuilder<TEntity>) tableDict.GetOrAdd(typeof(TEntity), _ => new EntityTypeBuilder<TEntity>());
        }

        private string CreateColumns<TEntity>(ColumnSelection selection)
        {
            return BuildColumns<TEntity>(
                x => IsSelected(x, selection),
                (sb, property) =>
                {
                    if (property.ComputedColumnSql != null)
                    {
                        sb.Append(property.ComputedColumnSql);
                        sb.Append(" AS ");
                        Adapter.EscapeColumnName(sb, property.Name);
                    }
                    else if (property.ColumnName != null && property.ColumnName != property.Name)
                    {
                        Adapter.EscapeColumnName(sb, property.ColumnName);
                        if (!selection.HasFlag(ColumnSelection.Write))
                        {
                            sb.Append(" AS ");
                            Adapter.EscapeColumnName(sb, property.Name);
                        }
                    }
                    else
                    {
                        Adapter.EscapeColumnName(sb, property.Name);
                    }
                });
        }

        private string CreateParams<TEntity>(ColumnSelection selection)
        {
            return BuildColumns<TEntity>(
                x => IsSelected(x, selection),
                (sb, property) =>
                {
                    if (property.ColumnType == null || !selection.HasFlag(ColumnSelection.Write))
                    {
                        sb.Append('@');
                        sb.Append(property.Name);
                    }
                    else
                    {
                        sb.Append("CAST(@");
                        sb.Append(property.Name);
                        sb.Append(" AS ");
                        sb.Append(property.ColumnType);
                        sb.Append(")");
                    }
                });
        }

        private string CreateColumnEqualParams<TEntity>(ColumnSelection selection)
        {
            return BuildColumns<TEntity>(
                x => IsSelected(x, selection),
                (sb, property) =>
                {
                    Adapter.EscapeColumnName(sb, property.ColumnName ?? property.Name);
                    if (property.ColumnType != null)
                    {
                        sb.Append("=CAST(@");
                        sb.Append(property.Name);
                        sb.Append(" AS ");
                        sb.Append(property.ColumnType);
                        sb.Append(")");
                    }
                    else
                    {
                        sb.Append("=@");
                        sb.Append(property.Name);
                    }
                });
        }

        private string BuildColumns<TEntity>(Func<PropertyBuilder, bool> predicate, Action<StringBuilder, PropertyBuilder> columnAction)
        {
            var entity = EnsureEntity<TEntity>();
            var properties = entity.GetProperties(this);
            var sb = new StringBuilder();
            foreach (var property in properties.Where(predicate))
            {
                if (sb.Length > 0)
                {
                    sb.Append(',');
                }

                columnAction(sb, property);
            }

            return sb.ToString();
        }

        private static bool IsSelected(PropertyBuilder property, ColumnSelection selection)
        {
            return !property.Ignored
                && ((property.IsKey && selection.HasFlag(ColumnSelection.Keys)) || (!property.IsKey && selection.HasFlag(ColumnSelection.NonKeys)))
                && (property.ComputedColumnSql == null || selection.HasFlag(ColumnSelection.Computed));
        }
    }
}