# Dapper.SqlGenerator
Database agnostic SQL code generation for Dapper without POCO attributes and Entity Framework Core compatible schema definition.

## Usage

Note: Documentation is not complete, check unit tests for more examples.

One of the project aims is performance - generated queries are cached as well as sets of columns.

Currently only NpgsqlConnection and SqlConnection are supported but writing a custom adapter is very simple and requires only implementing ISqlAdapter interface and registering it.

Project will aim to eventually be fully compatible with Entity Framework Core schema definition. To crate mappings for an existing database use the following command:

    dotnet ef dbcontext scaffold "Server=.\SQLEXPRESS;Database=MyDb;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models

In the generated schema definitions remove Entity Framework namespaces and initialize SqlGenerator with generated schema:

    using Dapper.SqlGenerator;
    using Dapper.SqlGenerator.Extensions;

    void InitSqlGenerator()
    {
        OnModelCreating(DapperSqlGenerator.Configure());
    }

Simplest use case does not require any initialization - just use the following methods on the IDbConnection:

    using Dapper.SqlGenerator;
    
    string insertSql = connection.Sql().Insert<Product>();
    string insertAndReturnSql = connection.Sql().InsertReturn<Product>();
    string updateSql = connection.Sql().Update<Product>();
    string deleteSql = connection.Sql().Delete<Product>();
    string upsertMergeSql = connection.Sql().Merge<Product>("unique_order");

    string tableName = connection.Sql().Table<Order>();

    string commaSeparatedNonKeyColumns = connection.Sql().GetColumns<Order>(ColumnSelection.NonKeys);
    string keysAtQueryParams = connection.Sql().GetParams<Order>(ColumnSelection.Keys);
    string columnEqualParams = connection.Sql().GetColumnEqualParams<Order>(ColumnSelection.NonKeys);
    
    DapperSqlGenerator.Configure().Entity<Order>.HasColumnSet("unique_order", x => x.OrderId, x => x.ProductId);
    IList<IProperty> uniqueColumns = connection.Sql().GetProperties<Order>(ColumnSelection.Select, 'unique_order');

There is no SELECT query available - GetColumns will generate comma generated columns.

On program initialization special naming or key column rules can be defined which will be handled by SqlGenerator.

Example for 2 entities:

    DapperSqlGenerator.Configure(connectionString)
        .HasDefaultKeyColumn("Id", o => o.HasColumnName("id"))
        .Entity<Product>(e =>
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
        .Entity<Order>(e =>
        {
            e.ToTable("orders");
            e.HasKey(c => c.OrderId);
            e.Property(c => c.OrderId)
                .HasColumnName("Id");
            e.HasColumnSet("unique_order", x => x.OrderId, x => x.ProductId);
        });

SqlGenerator can pick the correct database adapter based on the IDbConnection type or even optionally by it's connection string to handle selection of schema.

There are several table and column name converters available, all written for with realistic scenarios in mind so that there should be no need to use ToTable or HasColumnName specialzations very often.
Name converters can be joined together in any order to produce the expected outcome.

 - ```CamelCaseNameConverter``` - _PropertyName_ becomes _propertyName_
 - ```LowerCaseNameConverter``` - _PropertyName_ becomes _propertyname_
 - ```UpperCaseNameConverter``` - _PropertyName_ becomes _PROPERTYNAME_
 - ```SnakeCaseNameConverter``` - _PropertyNAME_ becomes _Property_NAME_
 - ```PluralNameConverter``` - _Grape_ becomes _Grapes_ but _Settings_ remains _Settings_

To register existing or custom adapter, use AdapterFactory:

    AdapterFactory.Register(
        typeof(NpgsqlConnection),
        new PostgresAdapter(new INameConverter[]
        {
            new SnakeCaseNameConverter(),
            new LowerCaseNameConverter(),
            new PluralNameConverter()
        },
        new INameConverter[]
        {
            new LowerCaseNameConverter()
        }
    );
