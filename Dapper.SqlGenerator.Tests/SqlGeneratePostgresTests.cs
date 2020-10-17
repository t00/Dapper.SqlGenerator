using Dapper.SqlGenerator.Tests.Connections;
using Dapper.SqlGenerator.Tests.TestClasses;
using NUnit.Framework;

namespace Dapper.SqlGenerator.Tests
{
    [TestFixture]
    public class SqlGeneratePostgresTests
    {
        [SetUp]
        public void Init()
        {
            ProductOrderInit.Init();
        }

        [TearDown]
        public void Finish()
        {
            ProductOrderInit.Reset();
        }
        
        [Test]
        public void TestGetColumnsProduct()
        {
            var connection = new NpgsqlConnection();
            var cols = connection.Sql().GetColumns<TestProduct>(ColumnSelection.Select);
            Assert.AreEqual("\"id\" AS \"Id\",\"Type\" AS \"Kind\",\"Content\",\"Id\" + 1 AS \"Value\",\"Enum\",\"MaybeDate\",\"Date\",\"MaybeGuid\",\"Guid\",\"Duration\",\"Last\"", cols);
            cols = connection.Sql().GetColumns<TestProduct>(ColumnSelection.Keys);
            Assert.AreEqual("\"id\" AS \"Id\"", cols, "Keys");
            cols = connection.Sql().GetColumns<TestProduct>(ColumnSelection.NonKeys | ColumnSelection.Computed);
            Assert.AreEqual("\"Type\" AS \"Kind\",\"Content\",\"Id\" + 1 AS \"Value\",\"Enum\",\"MaybeDate\",\"Date\",\"MaybeGuid\",\"Guid\",\"Duration\",\"Last\"", cols, "NonKeys");
        }

        [Test]
        public void TestGetColumnsOrder()
        {
            var connection = new NpgsqlConnection();
            var cols = connection.Sql().GetColumns<TestOrder>(ColumnSelection.Select);
            Assert.AreEqual("\"Id\" AS \"OrderId\",\"ProductId\",\"Count\"", cols);
            cols = connection.Sql().GetColumns<TestOrder>(ColumnSelection.Keys);
            Assert.AreEqual("\"Id\" AS \"OrderId\"", cols, "Keys");
            cols = connection.Sql().GetColumns<TestOrder>(ColumnSelection.NonKeys | ColumnSelection.Computed);
            Assert.AreEqual("\"ProductId\",\"Count\"", cols, "NonKeys");
        }

        [Test]
        public void TestGetColumnEqualParamProduct()
        {
            var connection = new NpgsqlConnection();
            var cols = connection.Sql().GetColumnEqualParams<TestProduct>(ColumnSelection.Keys | ColumnSelection.NonKeys);
            Assert.AreEqual("\"id\"=@Id,\"Type\"=@Kind,\"Content\"=CAST(@Content AS json),\"Enum\"=@Enum,\"MaybeDate\"=@MaybeDate,\"Date\"=@Date,\"MaybeGuid\"=@MaybeGuid,\"Guid\"=@Guid,\"Duration\"=@Duration,\"Last\"=@Last", cols);
            cols = connection.Sql().GetColumnEqualParams<TestProduct>(ColumnSelection.Keys);
            Assert.AreEqual("\"id\"=@Id", cols, "Keys");
            cols = connection.Sql().GetColumnEqualParams<TestProduct>(ColumnSelection.NonKeys);
            Assert.AreEqual("\"Type\"=@Kind,\"Content\"=CAST(@Content AS json),\"Enum\"=@Enum,\"MaybeDate\"=@MaybeDate,\"Date\"=@Date,\"MaybeGuid\"=@MaybeGuid,\"Guid\"=@Guid,\"Duration\"=@Duration,\"Last\"=@Last", cols, "NonKeys");
        }

        [Test]
        public void TestInsert()
        {
            var connection = new NpgsqlConnection();
            var insert = connection.Sql().Insert<TestProduct>();
            Assert.AreEqual("INSERT INTO \"TestProducts\" (\"Type\",\"Content\",\"Enum\",\"MaybeDate\",\"Date\",\"MaybeGuid\",\"Guid\",\"Duration\",\"Last\") VALUES (@Kind,CAST(@Content AS json),@Enum,@MaybeDate,@Date,@MaybeGuid,@Guid,@Duration,@Last)", insert);
        }

        [Test]
        public void TestInsertKeys()
        {
            var connection = new NpgsqlConnection();
            var insert = connection.Sql().Insert<TestProduct>(true);
            Assert.AreEqual("INSERT INTO \"TestProducts\" (\"id\",\"Type\",\"Content\",\"Enum\",\"MaybeDate\",\"Date\",\"MaybeGuid\",\"Guid\",\"Duration\",\"Last\") VALUES (@Id,@Kind,CAST(@Content AS json),@Enum,@MaybeDate,@Date,@MaybeGuid,@Guid,@Duration,@Last)", insert);
        }

        [Test]
        public void TestInsertReturn()
        {
            var connection = new NpgsqlConnection();
            var cols = connection.Sql().InsertReturn<TestProduct>();
            Assert.AreEqual("INSERT INTO \"TestProducts\" (\"Type\",\"Content\",\"Enum\",\"MaybeDate\",\"Date\",\"MaybeGuid\",\"Guid\",\"Duration\",\"Last\") VALUES (@Kind,CAST(@Content AS json),@Enum,@MaybeDate,@Date,@MaybeGuid,@Guid,@Duration,@Last) RETURNING \"id\" AS \"Id\"", cols);
        }

        [Test]
        public void TestInsertKeysReturn()
        {
            var connection = new NpgsqlConnection();
            var cols = connection.Sql().InsertReturn<TestProduct>(true);
            Assert.AreEqual("INSERT INTO \"TestProducts\" (\"id\",\"Type\",\"Content\",\"Enum\",\"MaybeDate\",\"Date\",\"MaybeGuid\",\"Guid\",\"Duration\",\"Last\") VALUES (@Id,@Kind,CAST(@Content AS json),@Enum,@MaybeDate,@Date,@MaybeGuid,@Guid,@Duration,@Last) RETURNING \"id\" AS \"Id\"", cols);
        }
        
        [Test]
        public void TestInsertOrders()
        {
            var connection = new NpgsqlConnection();
            var insert = connection.Sql().Insert<TestOrder>();
            Assert.AreEqual("INSERT INTO \"orders\" (\"ProductId\",\"Count\") VALUES (@ProductId,@Count)", insert);
        }

        [Test]
        public void TestInsertOrdersKeys()
        {
            var connection = new NpgsqlConnection();
            var insert = connection.Sql().Insert<TestOrder>(true);
            Assert.AreEqual("INSERT INTO \"orders\" (\"Id\",\"ProductId\",\"Count\") VALUES (@OrderId,@ProductId,@Count)", insert);
        }

        [Test]
        public void TestUpdate()
        {
            var connection = new NpgsqlConnection();
            var cols = connection.Sql().Update<TestProduct>();
            Assert.AreEqual("UPDATE \"TestProducts\" SET \"Type\"=@Kind,\"Content\"=CAST(@Content AS json),\"Enum\"=@Enum,\"MaybeDate\"=@MaybeDate,\"Date\"=@Date,\"MaybeGuid\"=@MaybeGuid,\"Guid\"=@Guid,\"Duration\"=@Duration,\"Last\"=@Last WHERE \"id\"=@Id", cols);
        }

        [Test]
        public void TestUpdateOrders()
        {
            var connection = new NpgsqlConnection();
            var cols = connection.Sql().Update<TestOrder>();
            Assert.AreEqual("UPDATE \"orders\" SET \"ProductId\"=@ProductId,\"Count\"=@Count WHERE \"Id\"=@OrderId", cols);
        }
        
        [Test]
        public void TestDelete()
        {
            var connection = new NpgsqlConnection();
            var cols = connection.Sql().Delete<TestProduct>();
            Assert.AreEqual("DELETE FROM \"TestProducts\" WHERE \"id\"=@Id", cols);
        }

        [Test]
        public void TestMerge()
        {
            var connection = new NpgsqlConnection();
            var cols = connection.Sql().Merge<TestOrder>("unique_order");
            Assert.AreEqual("INSERT INTO \"orders\" (\"ProductId\",\"Count\") VALUES (@ProductId,@Count) ON CONFLICT(\"Id\",\"ProductId\") DO UPDATE \"orders\" SET \"ProductId\"=@ProductId,\"Count\"=@Count WHERE \"Id\"=@OrderId AND \"ProductId\"=@ProductId", cols);
        }
        
        [Test]
        public void TestOrdersTable()
        {
            var connection = new NpgsqlConnection();
            var table = connection.Sql().Table<TestOrder>();
            Assert.AreEqual("\"orders\"", table);
        }
    }
}