using Dapper.SqlGenerator.Tests.Connections;
using Dapper.SqlGenerator.Tests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dapper.SqlGenerator.Tests
{
    [TestClass]
    public class SqlGenerateTests
    {
        [TestInitialize]
        public void Init()
        {
            ProductOrderInit.Init();
        }
        
        [TestMethod]
        public void TestGetColumnsProductPostgres()
        {
            var pgConnection = new NpgsqlConnection();
            var cols = pgConnection.Sql().GetColumns<TestProduct>(ColumnSelection.Select);
            Assert.AreEqual("\"id\" AS \"Id\",\"Type\" AS \"Kind\",\"Content\",\"Id\" + 1 AS \"Value\"", cols);
            cols = pgConnection.Sql().GetColumns<TestProduct>(ColumnSelection.Keys);
            Assert.AreEqual("\"id\" AS \"Id\"", cols, "Keys");
            cols = pgConnection.Sql().GetColumns<TestProduct>(ColumnSelection.NonKeys | ColumnSelection.Computed);
            Assert.AreEqual("\"Type\" AS \"Kind\",\"Content\",\"Id\" + 1 AS \"Value\"", cols, "NonKeys");
        }

        [TestMethod]
        public void TestGetColumnsProductSqlServer()
        {
            var sqlConnection = new SqlConnection();
            var cols = sqlConnection.Sql().GetColumns<TestProduct>(ColumnSelection.Select);
            Assert.AreEqual("[id] AS [Id],[Type] AS [Kind],[Content],[Id] + 1 AS [Value]", cols);
            cols = sqlConnection.Sql().GetColumns<TestProduct>(ColumnSelection.Keys);
            Assert.AreEqual("[id] AS [Id]", cols, "Keys");
            cols = sqlConnection.Sql().GetColumns<TestProduct>(ColumnSelection.NonKeys | ColumnSelection.Computed);
            Assert.AreEqual("[Type] AS [Kind],[Content],[Id] + 1 AS [Value]", cols, "NonKeys");
        }

        [TestMethod]
        public void TestGetColumnsOrderPostgres()
        {
            var pgConnection = new NpgsqlConnection();
            var cols = pgConnection.Sql().GetColumns<TestOrder>(ColumnSelection.Select);
            Assert.AreEqual("\"Id\" AS \"OrderId\",\"ProductId\",\"Count\"", cols);
            cols = pgConnection.Sql().GetColumns<TestOrder>(ColumnSelection.Keys);
            Assert.AreEqual("\"Id\" AS \"OrderId\"", cols, "Keys");
            cols = pgConnection.Sql().GetColumns<TestOrder>(ColumnSelection.NonKeys | ColumnSelection.Computed);
            Assert.AreEqual("\"ProductId\",\"Count\"", cols, "NonKeys");
        }

        [TestMethod]
        public void TestGetColumnsOrderSqlServer()
        {
            var pgConnection = new SqlConnection();
            var cols = pgConnection.Sql().GetColumns<TestOrder>(ColumnSelection.Select);
            Assert.AreEqual("[Id] AS [OrderId],[ProductId],[Count]", cols);
            cols = pgConnection.Sql().GetColumns<TestOrder>(ColumnSelection.Keys);
            Assert.AreEqual("[Id] AS [OrderId]", cols, "Keys");
            cols = pgConnection.Sql().GetColumns<TestOrder>(ColumnSelection.NonKeys | ColumnSelection.Computed);
            Assert.AreEqual("[ProductId],[Count]", cols, "NonKeys");
        }

        [TestMethod]
        public void TestGetColumnEqualParamProductPostgres()
        {
            var pgConnection = new NpgsqlConnection();
            var cols = pgConnection.Sql().GetColumnEqualParams<TestProduct>(ColumnSelection.Keys | ColumnSelection.NonKeys);
            Assert.AreEqual("\"id\"=@Id,\"Type\"=@Kind,\"Content\"=CAST(@Content AS json)", cols);
            cols = pgConnection.Sql().GetColumnEqualParams<TestProduct>(ColumnSelection.Keys);
            Assert.AreEqual("\"id\"=@Id", cols, "Keys");
            cols = pgConnection.Sql().GetColumnEqualParams<TestProduct>(ColumnSelection.NonKeys);
            Assert.AreEqual("\"Type\"=@Kind,\"Content\"=CAST(@Content AS json)", cols, "NonKeys");
        }

        [TestMethod]
        public void TestGetColumnEqualParamSqlServer()
        {
            var sqlConnection = new SqlConnection();
            var cols = sqlConnection.Sql().GetColumnEqualParams<TestProduct>(ColumnSelection.Keys | ColumnSelection.NonKeys);
            Assert.AreEqual("[id]=@Id,[Type]=@Kind,[Content]=@Content", cols);
            cols = sqlConnection.Sql().GetColumnEqualParams<TestProduct>(ColumnSelection.Keys);
            Assert.AreEqual("[id]=@Id", cols, "Keys");
            cols = sqlConnection.Sql().GetColumnEqualParams<TestProduct>(ColumnSelection.NonKeys);
            Assert.AreEqual("[Type]=@Kind,[Content]=@Content", cols, "NonKeys");
        }

        [TestMethod]
        public void TestInsertPostgres()
        {
            var pgConnection = new NpgsqlConnection();
            var insert = pgConnection.Sql().Insert<TestProduct>();
            Assert.AreEqual("INSERT INTO \"TestProducts\" (\"Type\",\"Content\") VALUES (@Kind,CAST(@Content AS json))", insert);
        }

        [TestMethod]
        public void TestInsertKeysPostgres()
        {
            var pgConnection = new NpgsqlConnection();
            var insert = pgConnection.Sql().Insert<TestProduct>(true);
            Assert.AreEqual("INSERT INTO \"TestProducts\" (\"id\",\"Type\",\"Content\") VALUES (@Id,@Kind,CAST(@Content AS json))", insert);
        }

        [TestMethod]
        public void TestInsertOrdersPostgres()
        {
            var pgConnection = new NpgsqlConnection();
            var insert = pgConnection.Sql().Insert<TestOrder>();
            Assert.AreEqual("INSERT INTO \"orders\" (\"ProductId\",\"Count\") VALUES (@ProductId,@Count)", insert);
        }

        [TestMethod]
        public void TestInsertOrdersKeysPostgres()
        {
            var pgConnection = new NpgsqlConnection();
            var insert = pgConnection.Sql().Insert<TestOrder>(true);
            Assert.AreEqual("INSERT INTO \"orders\" (\"Id\",\"ProductId\",\"Count\") VALUES (@OrderId,@ProductId,@Count)", insert);
        }

        [TestMethod]
        public void TestInsertSqlServer()
        {
            var sqlConnection = new SqlConnection();
            var insert = sqlConnection.Sql().Insert<TestProduct>();
            Assert.AreEqual("INSERT INTO [TestProducts] ([Type],[Content]) VALUES (@Kind,@Content)", insert);
        }

        [TestMethod]
        public void TestInsertKeysSqlServer()
        {
            var sqlConnection = new SqlConnection();
            var insert = sqlConnection.Sql().Insert<TestProduct>(true);
            Assert.AreEqual("INSERT INTO [TestProducts] ([id],[Type],[Content]) VALUES (@Id,@Kind,@Content)", insert);
        }

        [TestMethod]
        public void TestInsertReturnPostgres()
        {
            var pgConnection = new NpgsqlConnection();
            var cols = pgConnection.Sql().InsertReturn<TestProduct>();
            Assert.AreEqual("INSERT INTO \"TestProducts\" (\"Type\",\"Content\") VALUES (@Kind,CAST(@Content AS json)) RETURNING \"id\" AS \"Id\"", cols);
        }

        [TestMethod]
        public void TestInsertKeysReturnPostgres()
        {
            var pgConnection = new NpgsqlConnection();
            var cols = pgConnection.Sql().InsertReturn<TestProduct>(true);
            Assert.AreEqual("INSERT INTO \"TestProducts\" (\"id\",\"Type\",\"Content\") VALUES (@Id,@Kind,CAST(@Content AS json)) RETURNING \"id\" AS \"Id\"", cols);
        }

        [TestMethod]
        public void TestInsertReturnSqlServer()
        {
            var sqlConnection = new SqlConnection();
            var cols = sqlConnection.Sql().InsertReturn<TestProduct>();
            Assert.AreEqual("INSERT INTO [TestProducts] ([Type],[Content]) OUTPUT INSERTED.[id] AS [Id] VALUES (@Kind,@Content)", cols);
        }

        [TestMethod]
        public void TestInsertKeysReturnSqlServer()
        {
            var sqlConnection = new SqlConnection();
            var cols = sqlConnection.Sql().InsertReturn<TestProduct>(true);
            Assert.AreEqual("INSERT INTO [TestProducts] ([id],[Type],[Content]) OUTPUT INSERTED.[id] AS [Id] VALUES (@Id,@Kind,@Content)", cols);
        }

        [TestMethod]
        public void TestUpdatePostgres()
        {
            var pgConnection = new NpgsqlConnection();
            var cols = pgConnection.Sql().Update<TestProduct>();
            Assert.AreEqual("UPDATE \"TestProducts\" SET \"Type\"=@Kind,\"Content\"=CAST(@Content AS json) WHERE \"id\"=@Id", cols);
        }

        [TestMethod]
        public void TestUpdateOrdersPostgres()
        {
            var pgConnection = new NpgsqlConnection();
            var cols = pgConnection.Sql().Update<TestOrder>();
            Assert.AreEqual("UPDATE \"orders\" SET \"ProductId\"=@ProductId,\"Count\"=@Count WHERE \"Id\"=@OrderId", cols);
        }

        [TestMethod]
        public void TestUpdateSqlServer()
        {
            var sqlConnection = new SqlConnection();
            var cols = sqlConnection.Sql().Update<TestProduct>();
            Assert.AreEqual("UPDATE [TestProducts] SET [Type]=@Kind,[Content]=@Content WHERE [id]=@Id", cols);
        }

        [TestMethod]
        public void TestUpdateOrdersSqlServer()
        {
            var pgConnection = new SqlConnection();
            var cols = pgConnection.Sql().Update<TestOrder>();
            Assert.AreEqual("UPDATE [orders] SET [ProductId]=@ProductId,[Count]=@Count WHERE [Id]=@OrderId", cols);
        }

        [TestMethod]
        public void TestDeletePostgres()
        {
            var pgConnection = new NpgsqlConnection();
            var cols = pgConnection.Sql().Delete<TestProduct>();
            Assert.AreEqual("DELETE FROM \"TestProducts\" WHERE \"id\"=@Id", cols);
        }

        [TestMethod]
        public void TestDeleteSqlServer()
        {
            var sqlConnection = new SqlConnection();
            var cols = sqlConnection.Sql().Delete<TestProduct>();
            Assert.AreEqual("DELETE FROM [TestProducts] WHERE [id]=@Id", cols);
        }
    }
}
