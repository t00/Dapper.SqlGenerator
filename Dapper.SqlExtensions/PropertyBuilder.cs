namespace Dapper.SqlExtensions
{
    public class PropertyBuilder
    {
        public PropertyBuilder(string propertyName)
        {
            Name = propertyName;
        }
        
        public bool Ignored { get; set; }
        
        public bool IsKey { get; set; }
        
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

        public PropertyBuilder HasComputedColumnSql(string name)
        {
            ComputedColumnSql = name;
            return this;
        }
    }
}