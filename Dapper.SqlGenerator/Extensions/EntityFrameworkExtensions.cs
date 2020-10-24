using System;
using System.Linq;
using System.Linq.Expressions;

namespace Dapper.SqlGenerator.Extensions
{
    public static class EntityFrameworkExtensions
    {
        public static EntityTypeBuilder ToTable(this EntityTypeBuilder builder, string name)
        {
            builder.TableName = name;
            return builder;
        }

        public static EntityTypeBuilder<TEntity> ToTable<TEntity, TProperty>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, TProperty>> expression)
        {
            builder.TableName = Helpers.GetMemberName(expression);
            return builder;
        }

        public static PropertyBuilder Property<TEntity, TProperty>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, TProperty>> expression, Type adapter = null)
        {
            return builder.Property(Helpers.GetMemberName(expression), adapter);
        }

        public static void Ignore<TEntity, TProperty>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, TProperty>> expression, Type adapter = null)
        {
            builder.Property(Helpers.GetMemberName(expression), adapter).Ignored = true;
        }

        public static EntityTypeBuilder HasKey(this EntityTypeBuilder builder, params string[] names)
        {
            foreach (var name in names)
            {
                builder.Property(name).IsKey = true;
            }

            return builder;
        }

        public static EntityTypeBuilder<TEntity> HasKey<TEntity, TProperty>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, TProperty>> expression)
        {
            HasKey(builder, Helpers.GetMemberName(expression));
            return builder;
        }

        public static PropertyBuilder Ignore(this PropertyBuilder builder)
        {
            builder.Ignored = true;
            return builder;
        }

        public static PropertyBuilder HasColumnName(this PropertyBuilder builder, string name)
        {
            builder.ColumnName = name;
            return builder;
        }

        public static PropertyBuilder HasColumnType(this PropertyBuilder builder, string type)
        {
            builder.ColumnType = type;
            return builder;
        }

        public static PropertyBuilder HasNumericKey(this PropertyBuilder builder)
        {
            builder.IsNumeric = true;
            return builder;
        }

        public static PropertyBuilder HasComputedColumnSql(this PropertyBuilder builder, string name)
        {
            builder.ComputedColumnSql = name;
            return builder;
        }

    }
}