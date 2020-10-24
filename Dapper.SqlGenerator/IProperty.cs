namespace Dapper.SqlGenerator
{
    public interface IProperty
    {
        string Name { get; }
        
        string ColumnName { get; }

        string ColumnType { get; }

        bool IsKey { get; }
        
        bool Ignored { get; }
        
        /// <summary>
        /// The column represents a numeric key (byte or any int)
        /// </summary>
        bool IsNumeric { get; }
        
        string ComputedColumnSql { get; }
    }
}