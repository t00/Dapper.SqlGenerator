using System.Data;

namespace Dapper.SqlGenerator.Tests.Connections
{
    internal class SqlConnection : IDbConnection
    {
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public IDbTransaction BeginTransaction()
        {
            throw new System.NotImplementedException();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            throw new System.NotImplementedException();
        }

        public void ChangeDatabase(string databaseName)
        {
            throw new System.NotImplementedException();
        }

        public void Close()
        {
            throw new System.NotImplementedException();
        }

        public IDbCommand CreateCommand()
        {
            throw new System.NotImplementedException();
        }

        public void Open()
        {
            throw new System.NotImplementedException();
        }

        public string ConnectionString { get; set; } = "SqlServer CS";
            
        public int ConnectionTimeout { get; } = 1;

        public string Database { get; } = string.Empty;
            
        public ConnectionState State { get; } = ConnectionState.Broken;
    }
}