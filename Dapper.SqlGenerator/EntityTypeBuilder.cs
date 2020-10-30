using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Dapper.SqlGenerator
{
    public class EntityTypeBuilder
    {
        private readonly EntityTypeBuilder shared;

        public EntityTypeBuilder(EntityTypeBuilder shared)
        {
            this.shared = shared;
        }
        
        public string TableName { get; set; }
        
        internal ConcurrentDictionary<string, (PropertyBuilder shared, ConcurrentDictionary<Type, PropertyBuilder> adapters)> ColumnsDict { get; } = new ConcurrentDictionary<string, (PropertyBuilder, ConcurrentDictionary<Type, PropertyBuilder>)>();

        internal ConcurrentDictionary<string, IList<PropertyBuilder>> ColumnSetsDict { get; } = new ConcurrentDictionary<string, IList<PropertyBuilder>>();

        public PropertyBuilder FindProperty(string name, Type adapter = null)
        {
            if (!ColumnsDict.TryGetValue(name, out var found))
            {
                return null;
            }

            if (adapter == null)
            {
                return found.shared;
            }

            return found.adapters.TryGetValue(adapter, out var adapterColumn) ? adapterColumn : null;
        }
        
        public PropertyBuilder Property(string name, Type adapter = null)
        {
            var (sharedProperty, adapters) = ColumnsDict.GetOrAdd(name, _ =>
            {
                var foundShared = shared?.FindProperty(name, adapter); 
                return (foundShared != null ? new PropertyBuilder(foundShared) : new PropertyBuilder(name), new ConcurrentDictionary<Type, PropertyBuilder>());
            });
            
            return adapter == null ? sharedProperty : adapters.GetOrAdd(adapter, _ => new PropertyBuilder(sharedProperty));
        }
    }

    public class EntityTypeBuilder<TEntity> : EntityTypeBuilder
    {
        public EntityTypeBuilder(EntityTypeBuilder shared) : base(shared)
        {
        }
        
        public IEnumerable<PropertyBuilder> GetProperties(ModelBuilder modelBuilder)
        {
            var props = typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
            foreach (var prop in props.Where(modelBuilder.Adapter.AcceptType))
            {
                if (FindColumn(this, prop.Name, out var custom))
                {
                    yield return custom;
                }
                else if (FindColumn(modelBuilder.Shared, prop.Name, out var shared))
                {
                    yield return shared;
                }
                else
                {
                    yield return new PropertyBuilder(prop);
                }
            }

            bool FindColumn(EntityTypeBuilder builder, string propertyName, out PropertyBuilder property)
            {
                property = null;
                if (!builder.ColumnsDict.TryGetValue(propertyName, out var custom))
                {
                    return false;
                }
                
                var adapterType = modelBuilder.Adapter.GetType();
                if (custom.adapters.TryGetValue(adapterType, out property))
                {
                    return true;
                }

                property = custom.shared;
                return true;
            }
        }

        public EntityTypeBuilder<TEntity> HasColumnSet(string name, Type adapter, params string[] columns)
        {
            ColumnSetsDict[name] = columns.Select(x => Property(x, adapter)).ToList();
            return this;
        }
        
        public EntityTypeBuilder<TEntity> HasColumnSet(string name, params string[] columns)
        {
            return HasColumnSet(name, null, columns);
        }
        
        public EntityTypeBuilder<TEntity> HasColumnSet(string name, Type adapter, params Expression<Func<TEntity, object>>[] columns)
        {
            ColumnSetsDict[name] = columns.Select(x => Property(Helpers.GetMemberName(x), adapter)).ToList();
            return this;
        }
        
        public EntityTypeBuilder<TEntity> HasColumnSet(string name, params Expression<Func<TEntity, object>>[] columns)
        {
            return HasColumnSet(name, null, columns);
        }
    }
}