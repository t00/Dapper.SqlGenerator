using System.Data;
using System.Threading.Tasks;

namespace Dapper.SqlGenerator.Async
{
    public static class QueryExtensions
    {
        /// <summary>
        /// Execute the INSERT expression on the given table
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="entityToInsert">The object to INSERT.</param>
        /// <param name="columnSet">Set of columns to update</param>
        /// <param name="insertKeys">If true, keys will also be inserted.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <returns>Task executing UPDATE returning number of rows affected</returns>
        public static Task<int> InsertAsync<T>(this IDbConnection connection, T entityToInsert, string columnSet = null, bool insertKeys = false, IDbTransaction transaction = null, int? commandTimeout = null)
            =>  connection.ExecuteAsync(connection.Sql().Insert<T>(columnSet, insertKeys), entityToInsert, transaction, commandTimeout);
        
        /// <summary>
        /// Execute the INSERT expression on the given table
        /// RETURNING values of the keys defined for this table
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="entityToInsert">The object to INSERT.</param>
        /// <param name="columnSet">Set of columns to update</param>
        /// <param name="insertKeys">If true, keys will also be inserted.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <returns>Task executing INSERT returning object with inserted keys</returns>
        public static Task<T> InsertReturnAsync<T>(this IDbConnection connection, T entityToInsert, string columnSet = null, bool insertKeys = false, IDbTransaction transaction = null, int? commandTimeout = null)
            =>  connection.QuerySingleOrDefaultAsync<T>(connection.Sql().InsertReturn<T>(columnSet, insertKeys), entityToInsert, transaction, commandTimeout);

        /// <summary>
        /// Execute the INSERT expression on the given table
        /// RETURNING value of the first key column defined for this table
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <typeparam name="TScalar">Return type</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="entityToInsert">The object to INSERT.</param>
        /// <param name="columnSet">Set of columns to update</param>
        /// <param name="insertKeys">If true, keys will also be inserted.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <returns>Task executing INSERT returning object with inserted keys</returns>
        public static Task<TScalar> InsertReturnScalarAsync<TScalar, T>(this IDbConnection connection, T entityToInsert, string columnSet = null, bool insertKeys = false,
            IDbTransaction transaction = null, int? commandTimeout = null)
            => connection.ExecuteScalarAsync<TScalar>(connection.Sql().InsertReturn<T>(columnSet, insertKeys), entityToInsert, transaction, commandTimeout);

        /// <summary>
        /// Execute the UPDATE expression from the given table of records identified by keys 
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="entityToUpdate">The object to UPDATE containing all keys to filter on.</param>
        /// <param name="columnSet">Set of columns to update</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <returns>Task executing UPDATE returning number of rows affected</returns>
        public static Task<int> UpdateAsync<T>(this IDbConnection connection, T entityToUpdate, string columnSet = null, IDbTransaction transaction = null, int? commandTimeout = null)
            =>  connection.ExecuteAsync(connection.Sql().Update<T>(columnSet), entityToUpdate, transaction, commandTimeout);
        
        /// <summary>
        /// Execute the DELETE expression from the given table of records identified by keys 
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="entityToDelete">The object containing all keys for DELETE to filter on.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <returns>Task executing DELETE returning number of rows affected</returns>
        public static Task<int> DeleteAsync<T>(this IDbConnection connection, T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null)
            =>  connection.ExecuteAsync(connection.Sql().Delete<T>(), entityToDelete, transaction, commandTimeout);

        /// <summary>
        /// Execute the DELETE expression from the given table of records identified by keys 
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="param">The object containing all keys for DELETE to filter on.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <returns>Task executing DELETE returning number of rows affected</returns>
        public static Task<int> DeleteAsync<T>(this IDbConnection connection, object param, IDbTransaction transaction = null, int? commandTimeout = null)
            => connection.ExecuteAsync(connection.Sql().Delete<T>(), param, transaction, commandTimeout);

        /// <summary>
        /// Runs INSERT or UPDATE of records identified by a given set of columns
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <param name="connection">The connection to query on.</param>
        /// <param name="entity">The object to UPDATE containing all keys to filter on.</param>
        /// <param name="mergeSet">A set of columns to decide on when UPDATE instead of INSERT will be executed</param>
        /// <param name="insertKeys">If true, keys will also be inserted</param>
        /// <param name="columnSet">A set of columns to insert or update</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <returns>Task executing UPDATE returning number of rows affected</returns>
        public static Task<int> MergeAsync<T>(this IDbConnection connection, T entity, string mergeSet, string columnSet = null, bool insertKeys = false, IDbTransaction transaction = null, int? commandTimeout = null)
            =>  connection.ExecuteAsync(connection.Sql().Merge<T>(mergeSet, columnSet, insertKeys), entity, transaction, commandTimeout);
    }
}