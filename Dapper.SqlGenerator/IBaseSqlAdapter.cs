using System.Reflection;

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

        /// <summary>
        /// Gets escaped table name
        /// </summary>
        /// <param name="table">Table entity</param>
        /// <typeparam name="TEntity">Table entity type</typeparam>
        /// <returns>Escaped table name</returns>
        string GetTableName<TEntity>(EntityTypeBuilder<TEntity> table);
        
        /// <summary>
        /// Checks if column is included in selection
        /// </summary>
        /// <param name="property">Table property</param>
        /// <param name="selection">Column selection</param>
        /// <returns>True if included</returns>
        bool IsSelected(PropertyBuilder property, ColumnSelection selection);
        
        /// <summary>
        /// Gets column name or expression or null if column is not available to select or selection does not allow it
        /// </summary>
        /// <param name="property">Table property</param>
        /// <param name="selection">Column selection</param>
        /// <returns>Escaped column name or null if not available</returns>
        string GetColumn(PropertyBuilder property, ColumnSelection selection);
        
        /// <summary>
        /// Gets the @Parameter to substitute column value
        /// </summary>
        /// <param name="property">Table property</param>
        /// <param name="selection">Column selection</param>
        /// <returns>Parameter or null if not available</returns>
        string GetParam(PropertyBuilder property, ColumnSelection selection);
        
        /// <summary>
        /// Gets Column=@Parameter or Column=Sql Expression
        /// </summary>
        /// <param name="property">Table property</param>
        /// <param name="selection">Column selection</param>
        /// <returns>Expression or null if not available</returns>
        string GetColumnEqualParam(PropertyBuilder property, ColumnSelection selection);

        /// <summary>
        /// Returns true if the type on the object can be used in SQL expressions
        /// </summary>
        /// <param name="propertyInfo">Reflection property type information</param>
        /// <returns>True if property can be used</returns>
        bool AcceptType(PropertyInfo propertyInfo);
    }
}