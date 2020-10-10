using System;

namespace Dapper.SqlGenerator
{
    public class SqlGenerationException : Exception
    {
        public SqlGenerationException(string message) : base(message)
        {
            
        }
    }
}