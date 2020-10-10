using System.Collections.Generic;

namespace Dapper.SqlGenerator
{
    public interface ISql
    {
        IList<PropertyBuilder> GetProperties<TEntity>(ColumnSelection selection = ColumnSelection.Select);

        string GetColumns<TEntity>(ColumnSelection selection);

        string GetParams<TEntity>(ColumnSelection selection);

        string GetColumnEqualParams<TEntity>(ColumnSelection selection);

        string Insert<TEntity>(bool insertKeys = false);

        string InsertReturn<TEntity>(bool insertKeys = false);

        string Update<TEntity>();

        string Delete<TEntity>();
    }
}