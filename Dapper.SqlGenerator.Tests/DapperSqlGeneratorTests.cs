using System.Data;
using Dapper.SqlGenerator.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dapper.SqlGenerator.Tests
{
    [TestClass]
    public class DapperSqlGeneratorTests
    {
        [TestInitialize]
        public void Init()
        {
            DapperSqlGenerator
                .Configure()
                .HasDefaultKeyColumn("Id", o => o.HasColumnName("id"))
                .Entity<Product>(e =>
                {
                    e.ToTable("Products");
                    e.Property(x => x.Kind)
                        .HasColumnName("Type");
                    e.Property(x => x.Name)
                        .Ignore();
                    e.Property(x => x.Content, typeof(PostgresAdapter))
                        .HasColumnType("json");
                    e.Property(x => x.Value, typeof(PostgresAdapter))
                        .HasComputedColumnSql("\"Id\" + 1");
                    e.Property(x => x.Value, typeof(SqlServerAdapter))
                        .HasComputedColumnSql("[Id] + 1");
                })
                .Entity<Order>(e =>
                {
                    e.ToTable("orders");
                    e.HasKey(c => c.OrderId);
                    e.Property(c => c.OrderId)
                        .HasColumnName("Id");
                });
        }
        
        [TestMethod]
        public void TestGetColumnsProductPostgres()
        {
            var pgConnection = new NpgsqlConnection();
            var cols = pgConnection.Sql().GetColumns<Product>(ColumnSelection.Select);
            Assert.AreEqual("\"id\" AS \"Id\",\"Type\" AS \"Kind\",\"Content\",\"Id\" + 1 AS \"Value\"", cols);
            cols = pgConnection.Sql().GetColumns<Product>(ColumnSelection.Keys);
            Assert.AreEqual("\"id\" AS \"Id\"", cols, "Keys");
            cols = pgConnection.Sql().GetColumns<Product>(ColumnSelection.NonKeys | ColumnSelection.Computed);
            Assert.AreEqual("\"Type\" AS \"Kind\",\"Content\",\"Id\" + 1 AS \"Value\"", cols, "NonKeys");
        }

        [TestMethod]
        public void TestGetColumnsProductSqlServer()
        {
            var sqlConnection = new SqlConnection();
            var cols = sqlConnection.Sql().GetColumns<Product>(ColumnSelection.Select);
            Assert.AreEqual("[id] AS [Id],[Type] AS [Kind],[Content],[Id] + 1 AS [Value]", cols);
            cols = sqlConnection.Sql().GetColumns<Product>(ColumnSelection.Keys);
            Assert.AreEqual("[id] AS [Id]", cols, "Keys");
            cols = sqlConnection.Sql().GetColumns<Product>(ColumnSelection.NonKeys | ColumnSelection.Computed);
            Assert.AreEqual("[Type] AS [Kind],[Content],[Id] + 1 AS [Value]", cols, "NonKeys");
        }

        [TestMethod]
        public void TestGetColumnsOrderPostgres()
        {
            var pgConnection = new NpgsqlConnection();
            var cols = pgConnection.Sql().GetColumns<Order>(ColumnSelection.Select);
            Assert.AreEqual("\"Id\" AS \"OrderId\",\"ProductId\",\"Count\"", cols);
            cols = pgConnection.Sql().GetColumns<Order>(ColumnSelection.Keys);
            Assert.AreEqual("\"Id\" AS \"OrderId\"", cols, "Keys");
            cols = pgConnection.Sql().GetColumns<Order>(ColumnSelection.NonKeys | ColumnSelection.Computed);
            Assert.AreEqual("\"ProductId\",\"Count\"", cols, "NonKeys");
        }

        [TestMethod]
        public void TestGetColumnsOrderSqlServer()
        {
            var pgConnection = new SqlConnection();
            var cols = pgConnection.Sql().GetColumns<Order>(ColumnSelection.Select);
            Assert.AreEqual("[Id] AS [OrderId],[ProductId],[Count]", cols);
            cols = pgConnection.Sql().GetColumns<Order>(ColumnSelection.Keys);
            Assert.AreEqual("[Id] AS [OrderId]", cols, "Keys");
            cols = pgConnection.Sql().GetColumns<Order>(ColumnSelection.NonKeys | ColumnSelection.Computed);
            Assert.AreEqual("[ProductId],[Count]", cols, "NonKeys");
        }

        [TestMethod]
        public void TestGetColumnEqualParamProductPostgres()
        {
            var pgConnection = new NpgsqlConnection();
            var cols = pgConnection.Sql().GetColumnEqualParams<Product>(ColumnSelection.Keys | ColumnSelection.NonKeys);
            Assert.AreEqual("\"id\"=@Id,\"Type\"=@Kind,\"Content\"=CAST(@Content AS json)", cols);
            cols = pgConnection.Sql().GetColumnEqualParams<Product>(ColumnSelection.Keys);
            Assert.AreEqual("\"id\"=@Id", cols, "Keys");
            cols = pgConnection.Sql().GetColumnEqualParams<Product>(ColumnSelection.NonKeys);
            Assert.AreEqual("\"Type\"=@Kind,\"Content\"=CAST(@Content AS json)", cols, "NonKeys");
        }

        [TestMethod]
        public void TestGetColumnEqualParamSqlServer()
        {
            var sqlConnection = new SqlConnection();
            var cols = sqlConnection.Sql().GetColumnEqualParams<Product>(ColumnSelection.Keys | ColumnSelection.NonKeys);
            Assert.AreEqual("[id]=@Id,[Type]=@Kind,[Content]=@Content", cols);
            cols = sqlConnection.Sql().GetColumnEqualParams<Product>(ColumnSelection.Keys);
            Assert.AreEqual("[id]=@Id", cols, "Keys");
            cols = sqlConnection.Sql().GetColumnEqualParams<Product>(ColumnSelection.NonKeys);
            Assert.AreEqual("[Type]=@Kind,[Content]=@Content", cols, "NonKeys");
        }

        [TestMethod]
        public void TestInsertPostgres()
        {
            var pgConnection = new NpgsqlConnection();
            var insert = pgConnection.Sql().Insert<Product>();
            Assert.AreEqual("INSERT INTO \"Products\" (\"Type\",\"Content\") VALUES (@Kind,CAST(@Content AS json))", insert);
        }

        [TestMethod]
        public void TestInsertKeysPostgres()
        {
            var pgConnection = new NpgsqlConnection();
            var insert = pgConnection.Sql().Insert<Product>(true);
            Assert.AreEqual("INSERT INTO \"Products\" (\"id\",\"Type\",\"Content\") VALUES (@Id,@Kind,CAST(@Content AS json))", insert);
        }

        [TestMethod]
        public void TestInsertOrdersPostgres()
        {
            var pgConnection = new NpgsqlConnection();
            var insert = pgConnection.Sql().Insert<Order>();
            Assert.AreEqual("INSERT INTO \"orders\" (\"ProductId\",\"Count\") VALUES (@ProductId,@Count)", insert);
        }

        [TestMethod]
        public void TestInsertOrdersKeysPostgres()
        {
            var pgConnection = new NpgsqlConnection();
            var insert = pgConnection.Sql().Insert<Order>(true);
            Assert.AreEqual("INSERT INTO \"orders\" (\"Id\",\"ProductId\",\"Count\") VALUES (@OrderId,@ProductId,@Count)", insert);
        }

        [TestMethod]
        public void TestInsertSqlServer()
        {
            var sqlConnection = new SqlConnection();
            var insert = sqlConnection.Sql().Insert<Product>();
            Assert.AreEqual("INSERT INTO [Products] ([Type],[Content]) VALUES (@Kind,@Content)", insert);
        }

        [TestMethod]
        public void TestInsertKeysSqlServer()
        {
            var sqlConnection = new SqlConnection();
            var insert = sqlConnection.Sql().Insert<Product>(true);
            Assert.AreEqual("INSERT INTO [Products] ([id],[Type],[Content]) VALUES (@Id,@Kind,@Content)", insert);
        }

        [TestMethod]
        public void TestInsertReturnPostgres()
        {
            var pgConnection = new NpgsqlConnection();
            var cols = pgConnection.Sql().InsertReturn<Product>();
            Assert.AreEqual("INSERT INTO \"Products\" (\"Type\",\"Content\") VALUES (@Kind,CAST(@Content AS json)) RETURNING \"id\" AS \"Id\"", cols);
        }

        [TestMethod]
        public void TestInsertKeysReturnPostgres()
        {
            var pgConnection = new NpgsqlConnection();
            var cols = pgConnection.Sql().InsertReturn<Product>(true);
            Assert.AreEqual("INSERT INTO \"Products\" (\"id\",\"Type\",\"Content\") VALUES (@Id,@Kind,CAST(@Content AS json)) RETURNING \"id\" AS \"Id\"", cols);
        }

        [TestMethod]
        public void TestInsertReturnSqlServer()
        {
            var sqlConnection = new SqlConnection();
            var cols = sqlConnection.Sql().InsertReturn<Product>();
            Assert.AreEqual("INSERT INTO [Products] ([Type],[Content]) OUTPUT INSERTED.[id] AS [Id] VALUES (@Kind,@Content)", cols);
        }

        [TestMethod]
        public void TestInsertKeysReturnSqlServer()
        {
            var sqlConnection = new SqlConnection();
            var cols = sqlConnection.Sql().InsertReturn<Product>(true);
            Assert.AreEqual("INSERT INTO [Products] ([id],[Type],[Content]) OUTPUT INSERTED.[id] AS [Id] VALUES (@Id,@Kind,@Content)", cols);
        }

        [TestMethod]
        public void TestUpdatePostgres()
        {
            var pgConnection = new NpgsqlConnection();
            var cols = pgConnection.Sql().Update<Product>();
            Assert.AreEqual("UPDATE \"Products\" SET \"Type\"=@Kind,\"Content\"=CAST(@Content AS json) WHERE \"id\"=@Id", cols);
        }

        [TestMethod]
        public void TestUpdateOrdersPostgres()
        {
            var pgConnection = new NpgsqlConnection();
            var cols = pgConnection.Sql().Update<Order>();
            Assert.AreEqual("UPDATE \"orders\" SET \"ProductId\"=@ProductId,\"Count\"=@Count WHERE \"Id\"=@OrderId", cols);
        }

        [TestMethod]
        public void TestUpdateSqlServer()
        {
            var sqlConnection = new SqlConnection();
            var cols = sqlConnection.Sql().Update<Product>();
            Assert.AreEqual("UPDATE [Products] SET [Type]=@Kind,[Content]=@Content WHERE [id]=@Id", cols);
        }

        [TestMethod]
        public void TestUpdateOrdersSqlServer()
        {
            var pgConnection = new SqlConnection();
            var cols = pgConnection.Sql().Update<Order>();
            Assert.AreEqual("UPDATE [orders] SET [ProductId]=@ProductId,[Count]=@Count WHERE [Id]=@OrderId", cols);
        }

        [TestMethod]
        public void TestDeletePostgres()
        {
            var pgConnection = new NpgsqlConnection();
            var cols = pgConnection.Sql().Delete<Product>();
            Assert.AreEqual("DELETE FROM \"Products\" WHERE \"id\"=@Id", cols);
        }

        [TestMethod]
        public void TestDeleteSqlServer()
        {
            var sqlConnection = new SqlConnection();
            var cols = sqlConnection.Sql().Delete<Product>();
            Assert.AreEqual("DELETE FROM [Products] WHERE [id]=@Id", cols);
        }
        
        private class Product
        {
            public int Id { get; set; }
            
            public int Kind { get; set; }
            
            public string Name { get; set; }
            
            public string Content { get; set; }
            
            public int Value { get; set; }
        }

        private class Order
        {
            public int OrderId { get; set; }
            
            public int ProductId { get; set; }
            
            public int Count { get; set; }
        }

        private class SqlConnection : IDbConnection
        {
            public void Dispose()
            {
                throw new System.NotImplementedException();
            }

            public IDbTransaction BeginTransaction()
            {
                throw new System.NotImplementedException();
            }

            public IDbTransaction BeginTransaction(IsolationLevel il)
            {
                throw new System.NotImplementedException();
            }

            public void ChangeDatabase(string databaseName)
            {
                throw new System.NotImplementedException();
            }

            public void Close()
            {
                throw new System.NotImplementedException();
            }

            public IDbCommand CreateCommand()
            {
                throw new System.NotImplementedException();
            }

            public void Open()
            {
                throw new System.NotImplementedException();
            }

            public string ConnectionString { get; set; } = "SqlServer CS";
            
            public int ConnectionTimeout { get; } = 1;

            public string Database { get; } = string.Empty;
            
            public ConnectionState State { get; } = ConnectionState.Broken;
        }

        private class NpgsqlConnection : IDbConnection
        {
            public void Dispose()
            {
                throw new System.NotImplementedException();
            }

            public IDbTransaction BeginTransaction()
            {
                throw new System.NotImplementedException();
            }

            public IDbTransaction BeginTransaction(IsolationLevel il)
            {
                throw new System.NotImplementedException();
            }

            public void ChangeDatabase(string databaseName)
            {
                throw new System.NotImplementedException();
            }

            public void Close()
            {
                throw new System.NotImplementedException();
            }

            public IDbCommand CreateCommand()
            {
                throw new System.NotImplementedException();
            }

            public void Open()
            {
                throw new System.NotImplementedException();
            }

            public string ConnectionString { get; set; } = "Postgres CS";
            
            public int ConnectionTimeout { get; } = 1;

            public string Database { get; } = string.Empty;
            
            public ConnectionState State { get; } = ConnectionState.Broken;
        } 
    }
}
