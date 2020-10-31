using System.Data.SQLite;
using System.Reflection;
using Dapper.SqlGenerator.Async;
using NUnit.Framework;

namespace Dapper.SqlGenerator.Async.Tests
{
    [TestFixture]
    public class MigrationTests
    {
        [Test]
        public void TestMigrateSqlite()
        {
            using var connection = new SQLiteConnection(":memory:");
            connection.InitDatabase(Assembly.GetExecutingAssembly());
        }
    }
}