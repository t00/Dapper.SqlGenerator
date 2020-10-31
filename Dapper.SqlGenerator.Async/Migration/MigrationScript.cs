using System;
using System.Threading.Tasks;

namespace Dapper.SqlGenerator.Async.Migration
{
    public class MigrationScript
    {
        public string Name { get; set; }
        
        public string Extension { get; set; }
        
        public Func<Task<string>> GetContents { get; set; }
    }
}