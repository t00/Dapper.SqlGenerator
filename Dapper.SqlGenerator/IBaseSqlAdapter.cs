namespace Dapper.SqlGenerator
{
    public interface IBaseSqlAdapter
    {
        /// <summary>
        /// Adds the name of a column.
        /// </summary>
        /// <param name="name">The property name.</param>
        string EscapeColumnName(string name);

        /// <summary>
        /// Escape table name
        /// </summary>
        /// <param name="name">Table name</param>
        /// <returns>Escaped table name</returns>
        string EscapeTableName(string name);

        string GetTableName<TEntity>(EntityTypeBuilder<TEntity> table);
        
        bool IsSelected(PropertyBuilder property, ColumnSelection selection);
        
        string GetColumn(PropertyBuilder property, ColumnSelection selection);
        
        string GetParam(PropertyBuilder property, ColumnSelection selection);
        
        string GetColumnEqualParam(PropertyBuilder property, ColumnSelection selection);
    }
}