using System;
using System.Reflection;

namespace Dapper.SqlGenerator
{
    public class PropertyBuilder
    {
        public PropertyBuilder(string propertyName)
        {
            Name = propertyName;
        }

        public PropertyBuilder(PropertyInfo propertyInfo)
        {
            Name = propertyInfo.Name;
            var typeCode = Type.GetTypeCode(propertyInfo.PropertyType);
            IsNumeric = typeCode >= TypeCode.Byte && typeCode <= TypeCode.Int64;
        }

        public PropertyBuilder(PropertyBuilder source)
        {
            Name = source.Name;
            Ignored = source.Ignored;
            IsKey = source.IsKey;
            IsNumeric = source.IsNumeric;
            ColumnName = source.ColumnName;
            ColumnType = source.ColumnType;
            ComputedColumnSql = source.ComputedColumnSql;
        }

        public bool Ignored { get; set; }
        
        public bool IsKey { get; set; }
        
        /// <summary>
        /// The column represents a numeric key (byte or any int)
        /// </summary>
        public bool IsNumeric { get; set; }
        
        public string Name { get; }
        
        public string ColumnName { get; set; }

        public string ColumnType { get; set; }

        public string ComputedColumnSql { get; set; }

        public PropertyBuilder Ignore()
        {
            Ignored = true;
            return this;
        }

        public PropertyBuilder HasColumnName(string name)
        {
            ColumnName = name;
            return this;
        }

        public PropertyBuilder HasColumnType(string type)
        {
            ColumnType = type;
            return this;
        }

        public PropertyBuilder HasNumericKey()
        {
            IsNumeric = true;
            return this;
        }

        public PropertyBuilder HasComputedColumnSql(string name)
        {
            ComputedColumnSql = name;
            return this;
        }
    }
}