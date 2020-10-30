using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dapper.SqlGenerator
{
    public interface ISql
    {
        /// <summary>
        /// Gets the adapter associated with the schema definition
        /// </summary>
        ISqlAdapter Adapter { get; }

        /// <summary>
        /// Gets the escaped table name 
        /// </summary>
        /// <typeparam name="TEntity">Table type</typeparam>
        /// <returns>SQL column name</returns>
        string Table<TEntity>();

        ISql HasColumnSet<TEntity>(string name, Type adapter, params string[] columns);
        
        ISql HasColumnSet<TEntity>(string name, params string[] columns);

        ISql HasColumnSet<TEntity>(string name, Type adapter, params Expression<Func<TEntity, object>>[] columns);

        ISql HasColumnSet<TEntity>(string name, params Expression<Func<TEntity, object>>[] columns);

        /// <summary>
        /// Gets a list of columns present in the table
        /// </summary>
        /// <param name="selection">Column selection and purpose, see <see cref="ColumnSelection"/></param>
        /// <param name="columnSet">Named set of columns defined for a table</param>
        /// <typeparam name="TEntity">Table type</typeparam>
        /// <returns>List of columns with their properties</returns>
        IEnumerable<IProperty> GetProperties<TEntity>(string columnSet = null, ColumnSelection selection = ColumnSelection.Select);

        /// <summary>
        /// Gets a comma separated list of SQL column expressions for the table
        /// </summary>
        /// <param name="selection">Column selection and purpose, see <see cref="ColumnSelection"/></param>
        /// <param name="columnSet">Named set of columns defined for a table</param>
        /// <param name="alias">Table alias to prefix non-computed columns with</param>
        /// <param name="separator">Column separator, comma by default</param>
        /// <typeparam name="TEntity">Table type</typeparam>
        /// <returns>Comma separated list of </returns>
        string GetColumns<TEntity>(string columnSet = null, ColumnSelection selection = ColumnSelection.Select, string alias = null, string separator = ",");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selection">Column selection and purpose, see <see cref="ColumnSelection"/></param>
        /// <param name="columnSet">Named set of columns defined for a table</param>
        /// <param name="separator">Column separator, comma by default</param>
        /// <typeparam name="TEntity">Table type</typeparam>
        /// <returns></returns>
        string GetParams<TEntity>(string columnSet = null, ColumnSelection selection = ColumnSelection.Select, string separator = ",");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selection">Column selection and purpose, see <see cref="ColumnSelection"/></param>
        /// <param name="columnSet">Named set of columns defined for a table</param>
        /// <param name="alias">Table alias to prefix non-computed columns with</param>
        /// <param name="separator">Column separator, comma by default</param>
        /// <typeparam name="TEntity">Table type</typeparam>
        /// <returns></returns>
        string GetColumnEqualParams<TEntity>(string columnSet = null, ColumnSelection selection = ColumnSelection.Select, string alias = null, string separator = ",");

        /// <summary>
        /// Gets the SELECT query with an optional table alias
        /// </summary>
        /// <param name="columnSet">Named set of columns to select</param>
        /// <param name="alias">Optional table alias</param>
        /// <typeparam name="TEntity">Table type</typeparam>
        /// <returns>SQL SELECT expression</returns>
        string Select<TEntity>(string columnSet = null, string alias = null);

        /// <summary>
        /// Gets the SELECT query with WHERE from given key parameters, with an optional table alias
        /// </summary>
        /// <param name="columnSet">Named set of columns to select</param>
        /// <param name="alias">Optional table alias</param>
        /// <typeparam name="TEntity">Table type</typeparam>
        /// <returns>SQL SELECT expression</returns>
        string SelectSingle<TEntity>(string columnSet = null, string alias = null);

        /// <summary>
        /// Gets the SELECT query with WHERE from arbitrary column parameters, with an optional table alias
        /// </summary>
        /// <param name="whereSet">Named set of columns to filter by</param>
        /// <param name="columnSet">Named set of columns to select</param>
        /// <param name="alias">Optional table alias</param>
        /// <typeparam name="TEntity">Table type</typeparam>
        /// <returns>SQL SELECT expression</returns>
        string SelectWhere<TEntity>(string whereSet, string columnSet = null, string alias = null);

        /// <summary>
        /// Gets the INSERT expression on the given table with named parameters
        /// </summary>
        /// <param name="insertKeys">If true, keys will also be inserted</param>
        /// <param name="columnSet">Named set of columns to insert</param>
        /// <typeparam name="TEntity">Table type</typeparam>
        /// <returns>SQL INSERT expression</returns>
        string Insert<TEntity>(string columnSet = null, bool insertKeys = false);

        /// <summary>
        /// Gets the INSERT expression on the given table with named parameters
        /// returning values of the keys defined for this table
        /// </summary>
        /// <param name="insertKeys">If true, keys will also be inserted</param>
        /// <param name="columnSet">Named set of columns to insert</param>
        /// <typeparam name="TEntity">Table type</typeparam>
        /// <returns>SQL INSERT expression returning inserted key values</returns>
        string InsertReturn<TEntity>(string columnSet = null, bool insertKeys = false);

        /// <summary>
        /// Gets the UPDATE expression on the given table with named parameters of records identified by keys
        /// </summary>
        /// <param name="columnSet">Named set of columns to update</param>
        /// <typeparam name="TEntity">Table type</typeparam>
        /// <returns>SQL UPDATE expression</returns>
        string Update<TEntity>(string columnSet = null);

        /// <summary>
        /// Gets the DELETE expression from the given table of records identified by keys 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns>SQL DELETE expression</returns>
        string Delete<TEntity>();

        /// <summary>
        /// Gets the INSERT or UPDATE expression of records identified by a given set of columns
        /// </summary>
        /// <param name="mergeSet">A set of columns to decide on when UPDATE instead of INSERT will be executed</param>
        /// <param name="insertKeys">If true, keys will also be inserted</param>
        /// <param name="columnSet">A set of columns to insert or update</param>
        /// <typeparam name="TEntity">Table type</typeparam>
        /// <returns>INSERT OR UPDATE expression dependent on the database used</returns>
        string Merge<TEntity>(string mergeSet, string columnSet = null, bool insertKeys = false);
    }
}