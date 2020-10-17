using Dapper.SqlGenerator.Adapters;

namespace Dapper.SqlGenerator.Tests.TestClasses
{
    public static class ProductOrderInit
    {
        public static void Init(string connectionString = null)
        {
            DapperSqlGenerator
                .Configure(connectionString)
                .HasDefaultKeyColumn("Id", o => o.HasColumnName("id"))
                .Entity<TestProduct>(e =>
                {
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
                .Entity<TestOrder>(e =>
                {
                    e.ToTable("orders");
                    e.HasKey(c => c.OrderId);
                    e.Property(c => c.OrderId)
                        .HasColumnName("Id");
                    e.HasColumnSet("unique_order", x => x.OrderId, x => x.ProductId);
                });
        }

        public static void Reset(string connectionString = null)
        {
            DapperSqlGenerator.Reset(connectionString);
        }
    }
}