using System;
using System.Data;
using System.Data.SQLite;
using System.Reflection;
using System.Threading.Tasks;
using Dapper.SqlGenerator.Async.Migration;
using Dapper.SqlGenerator.Extensions;
using Dapper.SqlGenerator.Tests.TestClasses;
using NUnit.Framework;

namespace Dapper.SqlGenerator.Async.Tests
{
    public class QueryTestData
    {
        private const string ConnectionString = "Data Source=:memory:";

        public int Id1 { get; private set; }

        public int Id2 { get; private set; }

        public async Task<IDbConnection> SetUp()
        {
            var connection = new SQLiteConnection(ConnectionString);
            ProductOrderInit.Init(ConnectionString);
            DapperSqlGenerator.Configure(ConnectionString).Entity<TestProduct>(a =>
            {
                // Ignoring non-trivial mapping for testing
                a.Property(x => x.MaybeGuid).Ignore();
                a.Property(x => x.Guid).Ignore();
                a.Property(x => x.Duration).Ignore();
            });
            connection.Open();

            var namespaceName = Assembly.GetExecutingAssembly().GetName().Name + ".TestMigrations";
            var applied = await connection.InitDatabase(Assembly.GetExecutingAssembly(), namespaceName, new SimpleMigrationOptions { ForceApplyMissing = true });
            Assert.AreEqual(1, applied);

            var p1 = new TestProduct
            {
                Kind = 5,
                Name = "Product 1",
                Content = "Empty box",
                Value = -5,
                Enum = TestEnum.All,
                Date = DateTime.UtcNow
            };

            var inserted1 = await connection.InsertReturnAsync(p1);
            Id1 = inserted1.Id;
            Assert.AreNotEqual(0, Id1);

            var p2 = new TestProduct
            {
                Kind = 7,
                Name = "Product 2",
                Content = "Full box",
                Value = 987,
                Enum = TestEnum.None,
                Date = DateTime.UtcNow
            };
            
            var inserted2 = await connection.InsertReturnAsync(p2);
            Id2 = inserted2.Id;
            Assert.AreNotEqual(0, Id2);
            Assert.AreNotEqual(Id1, Id2);

            return connection;
        }

    }
}