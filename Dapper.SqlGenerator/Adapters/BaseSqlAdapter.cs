using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Dapper.SqlGenerator.Adapters
{
    public abstract class BaseSqlAdapter : IBaseSqlAdapter
    {
        private readonly INameConverter[] tableNameConverters;
        private readonly INameConverter[] columnNameConverters;
        private readonly ICollection<Type> nonPrimitiveTypes;

        protected BaseSqlAdapter(INameConverter[] tableNameConverters, INameConverter[] columnNameConverters, IEnumerable<Type> nonPrimitiveTypes = null)
        {
            this.tableNameConverters = tableNameConverters;
            this.columnNameConverters = columnNameConverters;
            this.nonPrimitiveTypes = nonPrimitiveTypes != null ? new HashSet<Type>(nonPrimitiveTypes) : new HashSet<Type>
            {
                typeof(decimal),
                typeof(string),
                typeof(DateTime),
                typeof(TimeSpan),
                typeof(Guid),
            }; 
        }

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

            var escapedColumnName = GetColumnName(property);
            if (escapedColumnName != escapedName && !selection.HasFlag(ColumnSelection.Write))
            {
                return $"{escapedColumnName} AS {escapedName}";
            }

            return escapedColumnName;
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
            var escapedColumnName = GetColumnName(property);
            if (property.ColumnType != null)
            {
                return $"{escapedColumnName}=CAST(@{property.Name} AS {property.ColumnType})";
            }
            else
            {
                return $"{escapedColumnName}=@{property.Name}";
            }
        }

        public virtual bool AcceptType(PropertyInfo property)
        {
            if (!property.CanWrite)
            {
                return false;
            }

            var type = property.PropertyType;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = Nullable.GetUnderlyingType(type) ?? type;
            }

            return type.IsPrimitive || type.IsEnum || nonPrimitiveTypes.Contains(type);
        }
        
        public string GetTableName<TEntity>(EntityTypeBuilder<TEntity> table)
        {
            if (table.TableName == null)
            {
                var name = typeof(TEntity).Name;
                foreach (var converter in tableNameConverters)
                {
                    name = converter.Convert(name);
                }

                table.TableName = name;
            }

            return EscapeTableName(table.TableName);
        }

        protected string GetColumnName(PropertyBuilder property)
        {
            if (property.ColumnName == null)
            {
                var name = property.Name;
                foreach (var converter in columnNameConverters)
                {
                    name = converter.Convert(name);
                }

                property.ColumnName = name;
            }

            return EscapeColumnName(property.ColumnName);
        }
        
        protected void AddInsert<TEntity>(StringBuilder sb, ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, bool insertKeys, string columnSet)
        {
            var selection = ColumnSelection.NonKeys | ColumnSelection.Write;
            if (insertKeys)
            {
                selection |= ColumnSelection.Keys;
            }

            sb.Append("INSERT INTO ");
            sb.Append(GetTableName(table));
            sb.Append(" (");
            sb.Append(modelBuilder.GetColumns<TEntity>(selection, columnSet));
            sb.Append(") VALUES (");
            sb.Append(modelBuilder.GetParams<TEntity>(selection, columnSet));
            sb.Append(")");
        }
        
        protected void AddUpdate<TEntity>(StringBuilder sb, ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, string columnSet = null)
        {
            sb.Append("UPDATE ");
            sb.Append(GetTableName(table));
            sb.Append(" SET ");
            sb.Append(modelBuilder.GetColumnEqualParams<TEntity>(ColumnSelection.NonKeys | ColumnSelection.Write, columnSet));
            sb.Append(" WHERE ");
            sb.Append(modelBuilder.GetColumnEqualParams<TEntity>(ColumnSelection.Keys | ColumnSelection.Write));
        }
        
        protected void AddDelete<TEntity>(StringBuilder sb, ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table)
        {
            sb.Append("DELETE FROM ");
            sb.Append(GetTableName(table));
            sb.Append(" WHERE ");
            sb.Append(modelBuilder.GetColumnEqualParams<TEntity>(ColumnSelection.Keys | ColumnSelection.Write));
        }
    }
}