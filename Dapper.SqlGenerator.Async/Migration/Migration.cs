using System;

namespace Dapper.SqlGenerator.Async.Migration
{
    public class Migration : IMigration
    {
        public string Name { get; set; }
        
        public DateTime Date { get; set; }
    }
}