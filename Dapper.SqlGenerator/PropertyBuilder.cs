using System;
using System.Reflection;

namespace Dapper.SqlGenerator
{
    public class PropertyBuilder : IProperty
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
            ColumnName = source.ColumnName;
            ColumnType = source.ColumnType;
            IsKey = source.IsKey;
            Ignored = source.Ignored;
            IsNumeric = source.IsNumeric;
            ComputedColumnSql = source.ComputedColumnSql;
        }

        public string Name { get; }
        
        public string ColumnName { get; set; }

        public string ColumnType { get; set; }

        public bool IsKey { get; set; }

        public bool Ignored { get; set; }
        
        /// <summary>
        /// The column represents a numeric key (byte or any int)
        /// </summary>
        public bool IsNumeric { get; set; }
        
        public string ComputedColumnSql { get; set; }
    }
}