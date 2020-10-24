using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dapper.SqlGenerator.Extensions
{
    public static class EntityFrameworkDummyExtensions
    {
        public static PropertyBuilder IsRequired(this PropertyBuilder builder)
        {
            return builder;
        }
        
        public static PropertyBuilder HasMaxLength(this PropertyBuilder builder, int maxLength)
        {
            return builder;
        }
        
        public static PropertyBuilder IsUnicode(this PropertyBuilder builder, bool isUnicode)
        {
            return builder;
        }
        
        public static PropertyBuilder HasDefaultValueSql(this PropertyBuilder builder, string sql)
        {
            return builder;
        }
        
        public static PropertyBuilder ValueGeneratedNever(this PropertyBuilder builder)
        {
            return builder;
        }
        
        public static IndexBuilder HasIndex<TEntity, TProperty>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, TProperty>> expression)
        {
            return IndexBuilder.Instance;
        }

        public class IndexBuilder
        {
            public static readonly IndexBuilder Instance = new IndexBuilder();
            
            public IndexBuilder HasName(string name)
            {
                return this;
            }
            
            public IndexBuilder IsUnique()
            {
                return this;
            }

            public IndexBuilder HasFilter(string filter)
            {
                return this;
            }
        }

        public static DiscriminatorBuilder<TType> HasDiscriminator<TType>(this EntityTypeBuilder builder, string name)
        {
            return DiscriminatorBuilder<TType>.Instance;
        }

        public class DiscriminatorBuilder<TType>
        {
            public static readonly DiscriminatorBuilder<TType> Instance = new DiscriminatorBuilder<TType>();
            
            public DiscriminatorBuilder<TType> HasValue<TEntity>(TType value)
            {
                return this;
            }
        }
        
        public static RelationBuilder<TEntity, TRelated> HasOne<TEntity, TRelated>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, TRelated>> expression)
        {
            return RelationBuilder<TEntity, TRelated>.Instance;
        }

        public static ReverseRelationBuilder<TEntity, TRelated> HasMany<TEntity, TRelated>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, IEnumerable<TRelated>>> expression)
        {
            return ReverseRelationBuilder<TEntity, TRelated>.Instance;
        }

        public class RelationBuilder<TEntity, TRelated>
        {
            public static readonly RelationBuilder<TEntity, TRelated> Instance = new RelationBuilder<TEntity, TRelated>();

            public RelationBuilder<TEntity, TRelated> WithMany()
            {
                return this;
            }
            
            public RelationBuilder<TEntity, TRelated> WithMany(Expression<Func<TRelated, IEnumerable<TEntity>>> expression)
            {
                return this;
            }

            public RelationBuilder<TEntity, TRelated> HasForeignKey<TProperty>(Expression<Func<TEntity, TProperty>> expression)
            {
                return this;
            }
            
            public RelationBuilder<TEntity, TRelated> HasConstraintName(string name)
            {
                return this;
            }

            public RelationBuilder<TEntity, TRelated> OnDelete(DeleteBehavior deleteBehavior)
            {
                return this;
            }
        }
        
        public class ReverseRelationBuilder<TEntity, TRelated>
        {
            public static readonly ReverseRelationBuilder<TEntity, TRelated> Instance = new ReverseRelationBuilder<TEntity, TRelated>();

            public RelationBuilder<TRelated, TEntity> WithOne(string name)
            {
                return RelationBuilder<TRelated, TEntity>.Instance;
            }
            
            public RelationBuilder<TRelated, TEntity> WithOne<TProperty>(Expression<Func<TRelated, TProperty>> expression)
            {
                return RelationBuilder<TRelated, TEntity>.Instance;
            }
        }
    }
    
    public enum DeleteBehavior
    {
        ClientSetNull,
        Restrict,
        SetNull,
        Cascade
    }
}