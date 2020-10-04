using System;
using System.Collections.Concurrent;
using System.Text;

namespace Dapper.SqlGenerator
{
    public class ModelBuilder
    {
        private readonly ConcurrentDictionary<Type, EntityTypeBuilder> tableDict = new ConcurrentDictionary<Type, EntityTypeBuilder>();
        private readonly ConcurrentDictionary<(Type type, ColumnSelection selection), string> columnsCache = new ConcurrentDictionary<(Type, ColumnSelection), string>();

        public ModelBuilder()
        {
            Shared = new EntityTypeBuilder(this);
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
            return (EntityTypeBuilder<TEntity>) tableDict.GetOrAdd(typeof(TEntity), _ => new EntityTypeBuilder<TEntity>(this));
        }
        
        private string CreateColumns<TEntity>(ColumnSelection selection)
        {
            var entity = EnsureEntity<TEntity>();
            var properties = entity.GetProperties(selection);
            var sb = new StringBuilder();
            foreach (var property in properties)
            {
                if (sb.Length > 0)
                {
                    sb.Append(',');
                }
                
                Adapter.AppendColumn(sb, property);
            }

            return sb.ToString();
        }
    }
}