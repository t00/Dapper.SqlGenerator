# Dapper.SqlGenerator
Database agnostic SQL code generation for Dapper without POCO attributes and Entity Framework Core compatible schema definition.

# Dapper.SqlGenerator.Async
Dapper database agnostic async SELECT, INSERT, UPDATE, DELETE, MERGE queries and a file or resource database migration tool

## Dapper.SqlGenerator Usage

Note: Documentation is not complete, check unit tests for more examples.

One of the project aims is performance - generated queries are cached as well as sets of columns.

Currently only NpgsqlConnection and SqlConnection are supported but writing a custom adapter is very simple and requires only implementing ISqlAdapter interface and registering it.

The Id column by default is assumed to be a key as it is rarely a case when generic Id property is not a key.

Simplest use case does not require any initialization - use the following methods on the IDbConnection:

    using Dapper.SqlGenerator;
    
    // returns INSERT query
    string insertSql = connection.Sql().Insert<Product>();
    
    // returns INSERT query which returns inserted key columns
    string insertAndReturnSql = connection.Sql().InsertReturn<Product>();
    
    // returns UPDATE query filtered by key columns
    string updateSql = connection.Sql().Update<Product>();
    
    // returns DELETE query filtered by key columns
    string deleteSql = connection.Sql().Delete<Product>();

    // returns escaped table name, using name converters for the connection database type
    string tableName = connection.Sql().Table<Order>();

    // returns comma separated list of non-key columns, including calculated columns
    string commaSeparatedNonKeyColumns = connection.Sql().GetColumns<Order>(ColumnSelection.NonKeys | ColumnSelection.Computed);
    
    // returns comma separated parameters for keys only
    string keysAtQueryParams = connection.Sql().GetParams<Order>(ColumnSelection.Keys);
    
    // returns comma separated assigmnents Column=@Column for non-key columns
    string columnEqualParams = connection.Sql().GetColumnEqualParams<Order>(ColumnSelection.NonKeys);
    
Using merge (also called upsert) needs a defined set of columns by which row uniqueness is determined:

    DapperSqlGenerator.Configure().Entity<Order>.HasColumnSet("unique_order", x => x.OrderId, x => x.ProductId);
    string upsertMergeSql = connection.Sql().Merge<Order>("unique_order");

Column sets can be used to narrow down the list of inserted or updated columns as well:

    IList<IProperty> uniqueColumns = connection.Sql().GetProperties<Order>(ColumnSelection.Select, "unique_order");
    string uniqueOrderColumns = connection.Sql().GetColumns<Order>(ColumnSelection.Select, "unique_order");

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

Project will aim to eventually be fully compatible with Entity Framework Core schema definition. To crate mappings for an existing database use the following command:

    dotnet ef dbcontext scaffold "Server=.\SQLEXPRESS;Database=MyDb;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models

In the generated schema definitions remove Entity Framework namespaces and initialize SqlGenerator with generated schema:

    using Dapper.SqlGenerator;
    using Dapper.SqlGenerator.Extensions;

    void InitSqlGenerator()
    {
        OnModelCreating(DapperSqlGenerator.Configure());
    }

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

# Dapper.SqlGenerator.Async Usage - Select

All typed Dapper Query variants are available, some example SELECT queries:

    using Dapper.SqlGenerator.Async;

    connection.Sql().HasColumnSet<Product>("id+kind+content", x => x.Id, x => x.Kind, x => x.Content);

    var all = await connection.SelectAsync<Product>();
    var allWithSomeColumns = await connection.SelectAsync<Product>("id+kind+content");
    var filtered = await connection.SelectWhereAsync<Product>("WHERE Kind > 5");
    var firstWithSomeColumns = await connection.SelectFirstAsync<Product>(new { Id = 1 }, "id+kind+content");
    var fisrtOrNull = connection.SelectFirstOrDefaultAsync<Product>(new { Id = -1 });
    var single =  = await connection.SelectSingleAsync<Product>(new { Id = 1 });
    var singleOrNull =  = await connection.SelectSingleOrDefaultAsync<Product>(new { Id = 1 });
  
# Dapper.SqlGenerator.Async Usage - INSERT, UPDATE, DELETE, MERGE
    
    using Dapper.SqlGenerator.Async;

    connection.Sql().HasColumnSet<Product>("id+kind+content", x => x.Id, x => x.Kind, x => x.Content);

    // note id column although in the set will not be inserted as it is a key
    var affectedRows = await connection.InsertAsync(new Product { Kind = 4, Content = "Square" }, "id+kind+content");
    // id will be inserted
    var affectedRowsInsertedId = await connection.InsertAsync(new Product { Id = 1234, Kind = 4, Content = "Circle" }, "id+kind+content", true);
    
    // inserted1.Id will have the newly inserted Id
    var inserted1 = await connection.InsertReturnAsync(new Product { Kind = 7, Content = "Triangle" });
    
    // only Kind and Content columns will be modified
    var affectedUpdateRows = await connection.UpdateAsync(new Product { Id = 1, Kind = 66, Content = "Modified" }, "id+kind+content");
    
    var affectedDeleteRows = await connection.DeleteAsync(new Product { Id = 2 });

    // Content column is considered unique
    connection.Sql().HasColumnSet<Product>("content", x => x.Content);
    connection.Sql().HasColumnSet<Product>("id+kind+content+value", x => x.Id, x => x.Kind, x => x.Content, x => x.Value);
    var affectedInsertOrUpdateRows = await connection.MergeAsync(new Product { Kind = 7, Content = "Triangle", Value = 123 }, "content", "id+kind+content+value");


    

# Dapper.SqlGenerator.Async Usage - Migrations

Resources containing SQL scripts must have an extension which is either "sql" (can be changed using options) or the name of ISqlAdapter class, lowercase i.e. "npgsqlconnection". Resource name should be prepared for correct ordering when applying migrations.

For example having folloging scripts:

    20200101-init-database.sql
    20200101-init-database.sqliteconnection
 
 - on SqliteConnection .sqliteconnection will execute first and .sql second, both in a same transaction
 - on any other connectionn, only .sql will execute
 - only one migration '20200101-init-database' will be added to the database when migration completed successfully

Example to prepare a database migration from scripts embedded as resources in MigrationResources namespace, for Sqlite:

    using Dapper.SqlGenerator.Async.Migration;
    
    var connectionString = "Data Source=:memory:";
    await using var connection = new SQLiteConnection(connectionString);
    connection.Open();

    var namespaceName = Assembly.GetExecutingAssembly().GetName().Name + ".MigrationResources";
    var noMigrationsApplied = await connection.InitDatabase(Assembly.GetExecutingAssembly(), namespaceName);



