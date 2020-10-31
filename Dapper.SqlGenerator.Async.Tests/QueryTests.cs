using System;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
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
            // Guids ignored due to issues with type conversion
            DapperSqlGenerator.Configure(connectionString).Entity<TestProduct>(a =>
            {
                // Ignoring for testing
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
                Date = DateTime.UtcNow,
                MaybeGuid = new Guid(),
                Guid = new Guid(),
                Duration = TimeSpan.FromHours(3)
            };

            var insertQuery = connection.Sql().InsertReturn<TestProduct>();
            var idProduct = await connection.QuerySingleAsync<TestProduct>(insertQuery, p1);
            Assert.AreNotEqual(p1.Id, idProduct.Id);
            p1.Id = idProduct.Id;

            var select = connection.Sql().Select<TestProduct>();
            var test = (await connection.QueryAsync(select)).ToList();

            var loadedP1 = await connection.QuerySingleAsync<TestProduct>(select, idProduct);
            Assert.AreEqual(p1.Kind, loadedP1.Kind);
            Assert.AreEqual(null, loadedP1.Name);
            Assert.AreEqual(p1.Content, loadedP1.Content);
            Assert.AreEqual(p1.Value, loadedP1.Value);
            Assert.AreEqual(p1.Enum, loadedP1.Enum);
            Assert.AreEqual(p1.Date, loadedP1.Date);
        }
    }
}