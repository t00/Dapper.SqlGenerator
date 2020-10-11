namespace Dapper.SqlGenerator.Adapters
{
    public class PostgresAdapter : GenericSqlAdapter
    {
        public PostgresAdapter(INameConverter[] tableNameConverters, INameConverter[] columnNameConverters) : base(tableNameConverters, columnNameConverters)
        {
        }
    }
}