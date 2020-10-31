using System;

namespace Dapper.SqlGenerator.Async.Migration
{
    public interface IMigration
    {
        string Name { get; set; }
        
        DateTime Date { get; set; }
    }
}