using System;
using System.Data;

namespace Dapper.SqlGenerator.Async.Migration
{
    public class MigrationOptions<TMigration>
    {
        public Func<ISqlAdapter, string> CheckHasMigrationsSql { get; set; } = adapter => "SELECT COUNT(1) FROM information_schema.tables WHERE table_name = @table";

        public Func<TMigration, ISqlAdapter, bool> UseTransaction { get; set; } = (migration, adapter) => true;

        public Action<TMigration, ISqlAdapter> BeforeAction { get; set; }
        
        public Action<TMigration, ISqlAdapter> AfterAction { get; set; }

        public string DefaultExtension { get; set; } = "sql";

        public Func<IDbConnection, string> GetExtension { get; set; } = connection => connection.GetType().Name.ToLower();
    }
}