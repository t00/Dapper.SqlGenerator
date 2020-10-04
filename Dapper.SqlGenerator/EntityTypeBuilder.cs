using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Dapper.SqlGenerator
{
    public class EntityTypeBuilder
    {
        private string tableName;
        
        protected ModelBuilder ModelBuilder { get; }
        
        internal ConcurrentDictionary<string, ConcurrentDictionary<Type, PropertyBuilder>> ColumnsDict { get; } = new ConcurrentDictionary<string, ConcurrentDictionary<Type, PropertyBuilder>>();

        public EntityTypeBuilder(ModelBuilder modelBuilder)
        {
            this.ModelBuilder = modelBuilder;
        }

        public EntityTypeBuilder ToTable(string name)
        {
            tableName = name;
            return this;
        }

        public PropertyBuilder Property(string name, Type adapter = null)
        {
            var values = ColumnsDict.GetOrAdd(name, _ => new ConcurrentDictionary<Type, PropertyBuilder>());
            return values.GetOrAdd(adapter ?? typeof(object), _ => new PropertyBuilder(name));
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
        public EntityTypeBuilder(ModelBuilder modelBuilder) : base(modelBuilder)
        {
        }

        public IEnumerable<PropertyBuilder> GetProperties(ColumnSelection selection)
        {
            var props = typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
            foreach (var prop in props)
            {
                if (FindColumn(this, prop.Name, out var custom))
                {
                    yield return custom;
                }
                else if (FindColumn(ModelBuilder.Shared, prop.Name, out var shared))
                {
                    yield return shared;
                }
                else
                {
                    yield return new PropertyBuilder(prop.Name)
                    {
                        ColumnName = prop.Name
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
                
                var adapterType = ModelBuilder.Adapter.GetType();
                if (custom.TryGetValue(adapterType, out property))
                {
                    return true;
                }

                if (custom.TryGetValue(typeof(object), out property))
                {
                    return true;
                }

                return false;
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