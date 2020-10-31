using System.Data.SQLite;
using System.Reflection;
using System.Threading.Tasks;
using Dapper.SqlGenerator.Async.Migration;
using Dapper.SqlGenerator.Tests.TestClasses;
using NUnit.Framework;

namespace Dapper.SqlGenerator.Async.Tests
{
    [TestFixture]
    public class MigrationTests
    {
        [Test]
        public async Task TestMigrateSqliteOnce()
        {
            var connectionString = "Data Source=:memory:;Version=3;New=True;";
            await using var connection = new SQLiteConnection(connectionString);
            ProductOrderInit.Init(connectionString);
            connection.Open();

            var namespaceName = Assembly.GetExecutingAssembly().GetName().Name + ".TestMigrations";
            var applied = await connection.InitDatabase(Assembly.GetExecutingAssembly(), namespaceName);
            Assert.AreEqual(1, applied);
            
            applied = await connection.InitDatabase(Assembly.GetExecutingAssembly(), namespaceName, new SimpleMigrationOptions { ForceApplyMissing = true });
            Assert.AreEqual(0, applied);
        }
    }
}