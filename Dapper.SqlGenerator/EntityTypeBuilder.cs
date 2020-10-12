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
        
        public string TableName { get; protected set; }
        
        internal ConcurrentDictionary<string, (PropertyBuilder shared, ConcurrentDictionary<Type, PropertyBuilder> adapters)> ColumnsDict { get; } = new ConcurrentDictionary<string, (PropertyBuilder, ConcurrentDictionary<Type, PropertyBuilder>)>();

        internal ConcurrentDictionary<string, IList<PropertyBuilder>> ColumnSetsDict { get; } = new ConcurrentDictionary<string, IList<PropertyBuilder>>();

        public EntityTypeBuilder ToTable(string name)
        {
            TableName = name;
            return this;
        }

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

        public EntityTypeBuilder HasKey(params string[] names)
        {
            foreach (var name in names)
            {
                Property(name).IsKey = true;
            }

            return this;
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
            foreach (var prop in props)
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
                    var typeCode = Type.GetTypeCode(prop.PropertyType);
                    yield return new PropertyBuilder(prop.Name)
                    {
                        ColumnName = prop.Name,
                        IsNumeric = typeCode >= TypeCode.Byte && typeCode <= TypeCode.Int64 
                    };
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
        
        public EntityTypeBuilder<TEntity> ToTable<TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            ToTable(GetMemberName(expression));
            return this;
        }

        public PropertyBuilder Property<TProperty>(Expression<Func<TEntity, TProperty>> expression, Type adapter = null)
        {
            return Property(GetMemberName(expression), adapter);
        }

        public void Ignore<TProperty>(Expression<Func<TEntity, TProperty>> expression, Type adapter = null)
        {
            Property(GetMemberName(expression), adapter).Ignore();
        }

        public EntityTypeBuilder<TEntity> HasKey<TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            HasKey(GetMemberName(expression));
            return this;
        }

        public EntityTypeBuilder<TEntity> HasColumnSet(string name, params Expression<Func<TEntity, object>>[] columns)
        {
            ColumnSetsDict[name] = columns.Select(x => Property(GetMemberName(x))).ToList();
            return this;
        }

        private static string GetMemberName<T>(Expression<T> expression)
        {
            switch (expression.Body)
            {
                case MemberExpression m:
                    return m.Member.Name;
                case UnaryExpression u when u.Operand is MemberExpression m:
                    return m.Member.Name;
                default:
                    throw new NotImplementedException(expression.GetType().ToString());
            }
        }
    }
}