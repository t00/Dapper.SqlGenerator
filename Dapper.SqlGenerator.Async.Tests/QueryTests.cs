using System;
using System.Data.SQLite;
using System.Reflection;
using System.Threading.Tasks;
using Dapper.SqlGenerator.Extensions;
using Dapper.SqlGenerator.Tests.TestClasses;
using NUnit.Framework;

namespace Dapper.SqlGenerator.Async.Tests
{
    [TestFixture]
    public class QueryTests
    {
        [Test]
        public async Task TestQueries()
        {
            var connectionString = "Data Source=:memory:;Version=3;New=True";
            await using var connection = new SQLiteConnection(connectionString);
            ProductOrderInit.Init(connectionString);
            DapperSqlGenerator.Configure(connectionString).Entity<TestProduct>(a =>
            {
                // Ignoring non-trivial mapping for testing
                a.Property(x => x.MaybeGuid).Ignore();
                a.Property(x => x.Guid).Ignore();
                a.Property(x => x.Duration).Ignore();
            });
            connection.Open();

            var namespaceName = Assembly.GetExecutingAssembly().GetName().Name + ".TestMigrations";
            var applied = await connection.InitDatabase(Assembly.GetExecutingAssembly(), namespaceName);
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

            var insertQuery = connection.Sql().InsertReturn<TestProduct>();
            var id1 = await connection.QuerySingleAsync<TestProduct>(insertQuery, p1);
            Assert.AreNotEqual(0, id1.Id);

            var p2 = new TestProduct
            {
                Kind = 7,
                Name = "Product 2",
                Content = "Full box",
                Value = 987,
                Enum = TestEnum.None,
                Date = DateTime.UtcNow
            };
            
            var id2 = await connection.QuerySingleAsync<TestProduct>(insertQuery, p2);
            Assert.AreNotEqual(0, id2.Id);
            Assert.AreNotEqual(id1.Id, id2.Id);
            
            var loadedP1 = await connection.SelectSingleAsync<TestProduct>(id1);
            Assert.AreEqual(p1.Kind, loadedP1.Kind);
            Assert.AreEqual(null, loadedP1.Name);
            Assert.AreEqual(p1.Content, loadedP1.Content);
            Assert.AreEqual(p1.Value, loadedP1.Value);
            Assert.AreEqual(p1.Enum, loadedP1.Enum);
            Assert.AreEqual(p1.Date, loadedP1.Date);
            
            var loadedP2 = await connection.SelectSingleAsync<TestProduct>(id2);
            Assert.AreEqual(p2.Kind, loadedP2.Kind);
            Assert.AreEqual(null, loadedP2.Name);
            Assert.AreEqual(p2.Content, loadedP2.Content);
            Assert.AreEqual(p2.Value, loadedP2.Value);
            Assert.AreEqual(p2.Enum, loadedP2.Enum);
            Assert.AreEqual(p2.Date, loadedP2.Date);
        }
    }
}