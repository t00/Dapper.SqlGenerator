using System;

namespace Dapper.SqlGenerator.Async.Migration
{
    public class SimpleMigration : IMigration
    {
        public string Name { get; set; }
        
        public DateTime Date { get; set; }
    }
}