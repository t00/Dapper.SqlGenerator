namespace Dapper.SqlGenerator.Adapters
{
    public class PostgresAdapter : GenericSqlAdapter
    {
        private const int MaxQuerySize = int.MaxValue;
        
        public PostgresAdapter(INameConverter[] tableNameConverters, INameConverter[] columnNameConverters) : base(tableNameConverters, columnNameConverters)
        {
        }
    }
}