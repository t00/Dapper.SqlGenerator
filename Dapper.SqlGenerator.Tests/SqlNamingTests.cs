using System.Collections.Generic;
using System.Data;
using Dapper.SqlGenerator.Adapters;
using Dapper.SqlGenerator.NameConverters;
using Dapper.SqlGenerator.Tests.Connections;
using Dapper.SqlGenerator.Tests.TestClasses;
using NUnit.Framework;

namespace Dapper.SqlGenerator.Tests
{
    [TestFixture]
    public class SqlNamingTests
    {
        private static readonly Dictionary<string, ISqlAdapter> TestAdapters = new Dictionary<string, ISqlAdapter>(6)
        {
            ["sqlconnection"] = new SqlServerAdapter(new INameConverter[] { new PluralNameConverter() }, new INameConverter[0]),
            ["npgsqlconnection"] = new PostgresAdapter(new INameConverter[] { new PluralNameConverter() }, new INameConverter[0]),
        };

        private static ISqlAdapter AdapterLookup(IDbConnection connection)
        {
            var adapterKey = connection.GetType().Name.ToLower();
            return TestAdapters.TryGetValue(adapterKey, out var adapter) ? adapter : new GenericSqlAdapter(new INameConverter[] { new PluralNameConverter() }, new INameConverter[0]);
        }
       
        [SetUp]
        public void Init()
        {
            ProductOrderInit.Init("new");
            AdapterFactory.AdapterLookup = AdapterLookup;
        }

        [TearDown]
        public void Cleanup()
        {
            AdapterFactory.AdapterLookup = null;
        }

        [Test]
        public void TestInsertPostgres()
        {
            TestAdapters["npgsqlconnection"] = new PostgresAdapter(new INameConverter[] { new SnakeCaseNameConverter(), new LowerCaseNameConverter(), new PluralNameConverter() }, new INameConverter[] { new LowerCaseNameConverter() });
            var pgConnection = new NpgsqlConnection() { ConnectionString = "new" };
            var insert = pgConnection.Sql().Insert<TestProduct>();
            Assert.AreEqual("INSERT INTO \"test_products\" (\"Type\",\"content\",\"enum\",\"maybedate\",\"date\",\"maybeguid\",\"guid\",\"duration\",\"last\") VALUES (@Kind,CAST(@Content AS json),@Enum,@MaybeDate,@Date,@MaybeGuid,@Guid,@Duration,@Last)", insert);
        }
    }
}