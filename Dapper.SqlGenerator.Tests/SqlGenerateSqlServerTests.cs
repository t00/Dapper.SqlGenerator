using Dapper.SqlGenerator.Tests.Connections;
using Dapper.SqlGenerator.Tests.TestClasses;
using NUnit.Framework;

namespace Dapper.SqlGenerator.Tests
{
    [TestFixture]
    public class SqlGenerateSqlServerTests
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
            var connection = new SqlConnection();
            var cols = connection.Sql().GetColumns<TestProduct>(ColumnSelection.Select);
            Assert.AreEqual("[id] AS [Id],[Type] AS [Kind],[Content],[Id] + 1 AS [Value],[Enum],[MaybeDate],[Date],[MaybeGuid],[Guid],[Duration],[Last]", cols);
            cols = connection.Sql().GetColumns<TestProduct>(ColumnSelection.Keys);
            Assert.AreEqual("[id] AS [Id]", cols, "Keys");
            cols = connection.Sql().GetColumns<TestProduct>(ColumnSelection.NonKeys | ColumnSelection.Computed);
            Assert.AreEqual("[Type] AS [Kind],[Content],[Id] + 1 AS [Value],[Enum],[MaybeDate],[Date],[MaybeGuid],[Guid],[Duration],[Last]", cols, "NonKeys");
        }

        [Test]
        public void TestGetColumnsOrder()
        {
            var connection = new SqlConnection();
            var cols = connection.Sql().GetColumns<TestOrder>(ColumnSelection.Select);
            Assert.AreEqual("[Id] AS [OrderId],[ProductId],[Count]", cols);
            cols = connection.Sql().GetColumns<TestOrder>(ColumnSelection.Keys);
            Assert.AreEqual("[Id] AS [OrderId]", cols, "Keys");
            cols = connection.Sql().GetColumns<TestOrder>(ColumnSelection.NonKeys | ColumnSelection.Computed);
            Assert.AreEqual("[ProductId],[Count]", cols, "NonKeys");
        }

        [Test]
        public void TestGetColumnEqualParam()
        {
            var connection = new SqlConnection();
            var cols = connection.Sql().GetColumnEqualParams<TestProduct>(ColumnSelection.Keys | ColumnSelection.NonKeys);
            Assert.AreEqual("[id]=@Id,[Type]=@Kind,[Content]=@Content,[Enum]=@Enum,[MaybeDate]=@MaybeDate,[Date]=@Date,[MaybeGuid]=@MaybeGuid,[Guid]=@Guid,[Duration]=@Duration,[Last]=@Last", cols);
            cols = connection.Sql().GetColumnEqualParams<TestProduct>(ColumnSelection.Keys);
            Assert.AreEqual("[id]=@Id", cols, "Keys");
            cols = connection.Sql().GetColumnEqualParams<TestProduct>(ColumnSelection.NonKeys);
            Assert.AreEqual("[Type]=@Kind,[Content]=@Content,[Enum]=@Enum,[MaybeDate]=@MaybeDate,[Date]=@Date,[MaybeGuid]=@MaybeGuid,[Guid]=@Guid,[Duration]=@Duration,[Last]=@Last", cols, "NonKeys");
        }

        [Test]
        public void TestInsert()
        {
            var connection = new SqlConnection();
            var insert = connection.Sql().Insert<TestProduct>();
            Assert.AreEqual("INSERT INTO [TestProducts] ([Type],[Content],[Enum],[MaybeDate],[Date],[MaybeGuid],[Guid],[Duration],[Last]) VALUES (@Kind,@Content,@Enum,@MaybeDate,@Date,@MaybeGuid,@Guid,@Duration,@Last)", insert);
        }

        [Test]
        public void TestInsertKeys()
        {
            var connection = new SqlConnection();
            var insert = connection.Sql().Insert<TestProduct>(true);
            Assert.AreEqual("INSERT INTO [TestProducts] ([id],[Type],[Content],[Enum],[MaybeDate],[Date],[MaybeGuid],[Guid],[Duration],[Last]) VALUES (@Id,@Kind,@Content,@Enum,@MaybeDate,@Date,@MaybeGuid,@Guid,@Duration,@Last)", insert);
        }

        [Test]
        public void TestInsertReturn()
        {
            var connection = new SqlConnection();
            var cols = connection.Sql().InsertReturn<TestProduct>();
            Assert.AreEqual("INSERT INTO [TestProducts] ([Type],[Content],[Enum],[MaybeDate],[Date],[MaybeGuid],[Guid],[Duration],[Last]) OUTPUT INSERTED.[id] AS [Id] VALUES (@Kind,@Content,@Enum,@MaybeDate,@Date,@MaybeGuid,@Guid,@Duration,@Last)", cols);
        }

        [Test]
        public void TestInsertKeysReturn()
        {
            var connection = new SqlConnection();
            var cols = connection.Sql().InsertReturn<TestProduct>(true);
            Assert.AreEqual("INSERT INTO [TestProducts] ([id],[Type],[Content],[Enum],[MaybeDate],[Date],[MaybeGuid],[Guid],[Duration],[Last]) OUTPUT INSERTED.[id] AS [Id] VALUES (@Id,@Kind,@Content,@Enum,@MaybeDate,@Date,@MaybeGuid,@Guid,@Duration,@Last)", cols);
        }

        [Test]
        public void TestInsertOrders()
        {
            var connection = new SqlConnection();
            var insert = connection.Sql().Insert<TestOrder>();
            Assert.AreEqual("INSERT INTO [orders] ([ProductId],[Count]) VALUES (@ProductId,@Count)", insert);
        }

        [Test]
        public void TestInsertOrdersKeys()
        {
            var connection = new SqlConnection();
            var insert = connection.Sql().Insert<TestOrder>(true);
            Assert.AreEqual("INSERT INTO [orders] ([Id],[ProductId],[Count]) VALUES (@OrderId,@ProductId,@Count)", insert);
        }

        [Test]
        public void TestUpdate()
        {
            var connection = new SqlConnection();
            var cols = connection.Sql().Update<TestProduct>();
            Assert.AreEqual("UPDATE [TestProducts] SET [Type]=@Kind,[Content]=@Content,[Enum]=@Enum,[MaybeDate]=@MaybeDate,[Date]=@Date,[MaybeGuid]=@MaybeGuid,[Guid]=@Guid,[Duration]=@Duration,[Last]=@Last WHERE [id]=@Id", cols);
        }

        [Test]
        public void TestUpdateOrders()
        {
            var connection = new SqlConnection();
            var cols = connection.Sql().Update<TestOrder>();
            Assert.AreEqual("UPDATE [orders] SET [ProductId]=@ProductId,[Count]=@Count WHERE [Id]=@OrderId", cols);
        }

        [Test]
        public void TestDelete()
        {
            var connection = new SqlConnection();
            var cols = connection.Sql().Delete<TestProduct>();
            Assert.AreEqual("DELETE FROM [TestProducts] WHERE [id]=@Id", cols);
        }
        
        [Test]
        public void TestMerge()
        {
            var connection = new SqlConnection();
            var cols = connection.Sql().Merge<TestOrder>("unique_order");
            Assert.AreEqual("SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;BEGIN TRAN;IF EXISTS (SELECT * FROM [orders] WITH (UPDLOCK) WHERE [Id]=@OrderId AND [ProductId]=@ProductId) UPDATE [orders] SET [ProductId]=@ProductId,[Count]=@Count WHERE [Id]=@OrderId; ELSE INSERT INTO [orders] ([ProductId],[Count]) VALUES (@ProductId,@Count); COMMIT", cols);
        }

        [Test]
        public void TestOrdersTable()
        {
            var connection = new SqlConnection();
            var table = connection.Sql().Table<TestOrder>();
            Assert.AreEqual("[orders]", table);
        }
    }
}