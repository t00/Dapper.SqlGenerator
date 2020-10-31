using System;
using System.Data;

namespace Dapper.SqlGenerator.Async.Migration
{
    public class MigrationOptions<TMigration>
    {
        public Func<TMigration, ISqlAdapter, bool> UseTransaction { get; set; } = (migration, adapter) => true;

        public Action<TMigration, ISqlAdapter> BeforeAction { get; set; }
        
        public Action<TMigration, ISqlAdapter> AfterAction { get; set; }

        public string DefaultExtension { get; set; } = "sql";

        public Func<IDbConnection, string> GetExtension { get; set; } = connection => connection.GetType().Name.ToLower();
        
        public bool ForceApplyMissing { get; set; }
    }
}