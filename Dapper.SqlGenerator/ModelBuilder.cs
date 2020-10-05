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

        public ModelBuilder HasDefaultIdColumn(string propertyName, Action<PropertyBuilder> options = null, Type adapter = null)
        {
            var property = Shared.Property(propertyName, adapter);
            options?.Invoke(property);
            return this;
        }

        public string GetColumns<TEntity>(ColumnSelection selection = ColumnSelection.All)
        {
            return columnsCache.GetOrAdd((typeof(TEntity), selection), key => CreateColumns<TEntity>(key.selection));
        }

        private EntityTypeBuilder<TEntity> EnsureEntity<TEntity>()
        {
            return (EntityTypeBuilder<TEntity>) tableDict.GetOrAdd(typeof(TEntity), _ => new EntityTypeBuilder<TEntity>());
        }
        
        private string CreateColumns<TEntity>(ColumnSelection selection)
        {
            var entity = EnsureEntity<TEntity>();
            var properties = entity.GetProperties(this, selection);
            var sb = new StringBuilder();
            foreach (var property in properties.Where(x => IsSelected(x, selection)))
            {
                if (sb.Length > 0)
                {
                    sb.Append(',');
                }

                if (property.ComputedColumnSql != null)
                {
                    sb.Append(property.ComputedColumnSql);
                    sb.Append(" AS ");
                }
                else if (property.ColumnName != null && property.ColumnName != property.Name)
                {
                    Adapter.EscapeColumnName(sb, property.ColumnName);
                    sb.Append(" AS ");
                }
                Adapter.EscapeColumnName(sb, property.Name);
            }

            return sb.ToString();
        }

        private static bool IsSelected(PropertyBuilder property, ColumnSelection selection)
        {
            return !property.Ignored && ((property.IsKey && selection.HasFlag(ColumnSelection.Keys)) || (!property.IsKey && selection.HasFlag(ColumnSelection.NonKeys)));
        }
    }
}