using System.Collections.Generic;

namespace Dapper.SqlGenerator
{
    public interface ISql
    {
        string Table<TEntity>();
        
        IList<PropertyBuilder> GetProperties<TEntity>(ColumnSelection selection = ColumnSelection.Select, string columnSet = null);

        string GetColumns<TEntity>(ColumnSelection selection, string columnSet = null);

        string GetParams<TEntity>(ColumnSelection selection, string columnSet = null);

        string GetColumnEqualParams<TEntity>(ColumnSelection selection, string columnSet = null);

        string Insert<TEntity>(bool insertKeys = false, string columnSet = null);

        string InsertReturn<TEntity>(bool insertKeys = false, string columnSet = null);

        string Update<TEntity>(string columnSet = null);

        string Delete<TEntity>();

        string Merge<TEntity>(string mergeSet, bool insertKeys = false, string columnSet = null);
    }
}