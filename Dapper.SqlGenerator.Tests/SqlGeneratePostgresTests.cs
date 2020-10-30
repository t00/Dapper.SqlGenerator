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
            var cols = connection.Sql().GetColumns<TestProduct>(null, ColumnSelection.Select);
            Assert.AreEqual("\"id\" AS \"Id\",\"Type\" AS \"Kind\",\"Content\",\"Id\" + 1 AS \"Value\",\"Enum\",\"MaybeDate\",\"Date\",\"MaybeGuid\",\"Guid\",\"Duration\",\"Last\"", cols);
            cols = connection.Sql().GetColumns<TestProduct>(null, ColumnSelection.Keys);
            Assert.AreEqual("\"id\" AS \"Id\"", cols, "Keys");
            cols = connection.Sql().GetColumns<TestProduct>(null, ColumnSelection.NonKeys | ColumnSelection.Computed);
            Assert.AreEqual("\"Type\" AS \"Kind\",\"Content\",\"Id\" + 1 AS \"Value\",\"Enum\",\"MaybeDate\",\"Date\",\"MaybeGuid\",\"Guid\",\"Duration\",\"Last\"", cols, "NonKeys");
        }

        [Test]
        public void TestGetColumnsOrder()
        {
            var connection = new NpgsqlConnection();
            var cols = connection.Sql().GetColumns<TestOrder>(null, ColumnSelection.Select);
            Assert.AreEqual("\"Id\" AS \"OrderId\",\"ProductId\",\"Count\"", cols);
            cols = connection.Sql().GetColumns<TestOrder>(null, ColumnSelection.Keys);
            Assert.AreEqual("\"Id\" AS \"OrderId\"", cols, "Keys");
            cols = connection.Sql().GetColumns<TestOrder>(null, ColumnSelection.NonKeys | ColumnSelection.Computed);
            Assert.AreEqual("\"ProductId\",\"Count\"", cols, "NonKeys");
        }

        [Test]
        public void TestGetColumnEqualParamProduct()
        {
            var connection = new NpgsqlConnection();
            var cols = connection.Sql().GetColumnEqualParams<TestProduct>(null, ColumnSelection.Keys | ColumnSelection.NonKeys);
            Assert.AreEqual("\"id\"=@Id,\"Type\"=@Kind,\"Content\"=CAST(@Content AS json),\"Enum\"=@Enum,\"MaybeDate\"=@MaybeDate,\"Date\"=@Date,\"MaybeGuid\"=@MaybeGuid,\"Guid\"=@Guid,\"Duration\"=@Duration,\"Last\"=@Last", cols);
            cols = connection.Sql().GetColumnEqualParams<TestProduct>(null, ColumnSelection.Keys);
            Assert.AreEqual("\"id\"=@Id", cols, "Keys");
            cols = connection.Sql().GetColumnEqualParams<TestProduct>(null, ColumnSelection.NonKeys);
            Assert.AreEqual("\"Type\"=@Kind,\"Content\"=CAST(@Content AS json),\"Enum\"=@Enum,\"MaybeDate\"=@MaybeDate,\"Date\"=@Date,\"MaybeGuid\"=@MaybeGuid,\"Guid\"=@Guid,\"Duration\"=@Duration,\"Last\"=@Last", cols, "NonKeys");
        }

        [Test]
        public void TestSelect()
        {
            var connection = new NpgsqlConnection();
            var select = connection.Sql().Select<TestProduct>();
            Assert.AreEqual("SELECT \"id\" AS \"Id\",\"Type\" AS \"Kind\",\"Content\",\"Id\" + 1 AS \"Value\",\"Enum\",\"MaybeDate\",\"Date\",\"MaybeGuid\",\"Guid\",\"Duration\",\"Last\" FROM \"TestProducts\"", select);
        }

        [Test]
        public void TestSelectAlias()
        {
            var connection = new NpgsqlConnection();
            var select = connection.Sql().Select<TestProduct>(null, "ko");
            Assert.AreEqual("SELECT ko.\"id\" AS \"Id\",ko.\"Type\" AS \"Kind\",ko.\"Content\",\"Id\" + 1 AS \"Value\",ko.\"Enum\",ko.\"MaybeDate\",ko.\"Date\",ko.\"MaybeGuid\",ko.\"Guid\",ko.\"Duration\",ko.\"Last\" FROM \"TestProducts\" ko", select);
        }

        [Test]
        public void TestSelectColumnSet()
        {
            var connection = new NpgsqlConnection();
            connection.Sql().HasColumnSet<TestProduct>("TestSelectColumnSet", x => x.Id, x => x.Kind, x => x.Value, x => x.Content, x => x.Last);
            var select = connection.Sql().Select<TestProduct>("TestSelectColumnSet");
            Assert.AreEqual("SELECT \"id\" AS \"Id\",\"Type\" AS \"Kind\",\"Content\",\"Id\" + 1 AS \"Value\",\"Last\" FROM \"TestProducts\"", select);
        }

        [Test]
        public void TestSelectSingle()
        {
            var connection = new NpgsqlConnection();
            var select = connection.Sql().SelectSingle<TestProduct>();
            Assert.AreEqual("SELECT \"id\" AS \"Id\",\"Type\" AS \"Kind\",\"Content\",\"Id\" + 1 AS \"Value\",\"Enum\",\"MaybeDate\",\"Date\",\"MaybeGuid\",\"Guid\",\"Duration\",\"Last\" FROM \"TestProducts\" WHERE \"id\"=@Id", select);
        }

        [Test]
        public void TestSelectSingleSetAlias()
        {
            var connection = new NpgsqlConnection();
            connection.Sql().HasColumnSet<TestProduct>("TestSelectSingleSetAlias", x => x.Kind, x => x.Value, x => x.Content, x => x.Last);
            var select = connection.Sql().SelectSingle<TestProduct>("TestSelectSingleSetAlias", "ss");
            Assert.AreEqual("SELECT ss.\"Type\" AS \"Kind\",ss.\"Content\",\"Id\" + 1 AS \"Value\",ss.\"Last\" FROM \"TestProducts\" ss WHERE ss.\"id\"=@Id", select);
        }

        [Test]
        public void TestSelectWhere()
        {
            var connection = new NpgsqlConnection();
            connection.Sql().HasColumnSet<TestProduct>("TestSelectWhere", x => x.Id, x => x.Kind);
            connection.Sql().HasColumnSet<TestProduct>("TestSelectWhere_SELECT", x => x.Id, x => x.Kind, x => x.Value, x => x.Content, x => x.Last);
            var select = connection.Sql().SelectWhere<TestProduct>("TestSelectWhere", "TestSelectWhere_SELECT", "x");
            Assert.AreEqual("SELECT x.\"id\" AS \"Id\",x.\"Type\" AS \"Kind\",x.\"Content\",\"Id\" + 1 AS \"Value\",x.\"Last\" FROM \"TestProducts\" x WHERE x.\"id\"=@Id AND x.\"Type\"=@Kind", select);
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
            var insert = connection.Sql().Insert<TestProduct>(null, true);
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
            var cols = connection.Sql().InsertReturn<TestProduct>(null, true);
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
            var insert = connection.Sql().Insert<TestOrder>(null, true);
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