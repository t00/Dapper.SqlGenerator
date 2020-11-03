using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Dapper.SqlGenerator.Async
{
    public static class SelectExtensions
    {
        /// <summary>
        /// Execute a SELECT query asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">The type of results to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="columnSet">Set of columns to select</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>
        /// A sequence of data of <typeparamref name="T"/>; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        public static Task<IEnumerable<T>> SelectAsync<T>(this IDbConnection cnn, string columnSet = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
            => cnn.QueryAsync<T>(cnn.Sql().Select<T>(columnSet), null, transaction, commandTimeout, commandType);

        /// <summary>
        /// Execute a SELECT WHERE query asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">The type of results to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="suffix">SELECT FROM query suffix, can contain WHERE, ORDER BY or other expressions.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="columnSet">Set of columns to select</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>
        /// A sequence of data of <typeparamref name="T"/>; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        public static Task<IEnumerable<T>> SelectWhereAsync<T>(this IDbConnection cnn, string suffix = null, object param = null, string columnSet = null,
            IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var sql = cnn.Sql().Select<T>(columnSet);
            sql = string.IsNullOrWhiteSpace(suffix) ? sql : $"{sql} {suffix}";
            return cnn.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Execute a single-row SELECT query asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">The type of result to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="columnSet">Set of columns to select</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public static Task<T> SelectFirstAsync<T>(this IDbConnection cnn, object param = null, string columnSet = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null)
            => cnn.QueryFirstAsync<T>(cnn.Sql().SelectSingle<T>(columnSet), param, transaction, commandTimeout, commandType);

        /// <summary>
        /// Execute a single-row SELECT query asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">The type of result to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="param">The parameters to pass, if keys are defined</param>
        /// <param name="columnSet">Set of columns to select</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public static Task<T> SelectFirstAsync<T>(this IDbConnection cnn, T param = null, string columnSet = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null) where T : class
            => cnn.QueryFirstAsync<T>(cnn.Sql().SelectSingle<T>(columnSet), param, transaction, commandTimeout, commandType);

        /// <summary>
        /// Execute a single-row SELECT query asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">The type of result to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="param">The parameters to pass, if keys are defined</param>
        /// <param name="columnSet">Set of columns to select</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public static Task<T> SelectFirstOrDefaultAsync<T>(this IDbConnection cnn, object param = null, string columnSet = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null) where T : class
            => cnn.QueryFirstOrDefaultAsync<T>(cnn.Sql().SelectSingle<T>(columnSet), param, transaction, commandTimeout, commandType);

        /// <summary>
        /// Execute a single-row SELECT query asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">The type of result to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="param">The parameters to pass, if keys are defined</param>
        /// <param name="columnSet">Set of columns to select</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public static Task<T> SelectFirstOrDefaultAsync<T>(this IDbConnection cnn, T param = null, string columnSet = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null) where T : class
            => cnn.QueryFirstOrDefaultAsync<T>(cnn.Sql().SelectSingle<T>(columnSet), param, transaction, commandTimeout, commandType);

        /// <summary>
        /// Execute a single-row SELECT query asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">The type of result to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="param">The parameters to pass, if keys are defined</param>
        /// <param name="columnSet">Set of columns to select</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public static Task<T> SelectSingleAsync<T>(this IDbConnection cnn, object param = null, string columnSet = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null) =>
            cnn.QuerySingleAsync<T>(cnn.Sql().SelectSingle<T>(columnSet), param, transaction, commandTimeout, commandType);

        /// <summary>
        /// Execute a single-row SELECT query asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">The type of result to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="param">The parameters to pass, if keys are defined</param>
        /// <param name="columnSet">Set of columns to select</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public static Task<T> SelectSingleAsync<T>(this IDbConnection cnn, T param = null, string columnSet = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null) where T : class
            => cnn.QuerySingleAsync<T>(cnn.Sql().SelectSingle<T>(columnSet), param, transaction, commandTimeout, commandType);

        /// <summary>
        /// Execute a single-row SELECT query asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="param">The parameters to pass, if keys are defined</param>
        /// <param name="columnSet">Set of columns to select</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public static Task<T> SelectSingleOrDefaultAsync<T>(this IDbConnection cnn, object param = null, string columnSet = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null) =>
            cnn.QuerySingleOrDefaultAsync<T>(cnn.Sql().SelectSingle<T>(columnSet), param, transaction, commandTimeout, commandType);
        
        /// <summary>
        /// Execute a single-row SELECT query asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="param">The parameters to pass, if keys are defined</param>
        /// <param name="columnSet">Set of columns to select</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public static Task<T> SelectSingleOrDefaultAsync<T>(this IDbConnection cnn, T param = null, string columnSet = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null) where T : class
            => cnn.QuerySingleOrDefaultAsync<T>(cnn.Sql().SelectSingle<T>(columnSet), param, transaction, commandTimeout, commandType);
    }
}