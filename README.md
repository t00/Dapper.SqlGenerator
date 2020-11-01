# Dapper.SqlGenerator
Database agnostic SQL code generation for Dapper without POCO attributes and Entity Framework Core compatible schema definition.

[Usage and examples](https://github.com/t00/Dapper.SqlGenerator/tree/main/Dapper.SqlGenerator)

# Dapper.SqlGenerator.Async
Dapper database agnostic async SELECT, INSERT, UPDATE, DELETE, MERGE queries and a file or resource database migration tool

[Usage and examples](https://github.com/t00/Dapper.SqlGenerator/tree/main/Dapper.SqlGenerator.Async)

# Dapper.SqlGenerator Quick Start

Create model from db:

    dotnet ef dbcontext scaffold "Server=.\SQLEXPRESS;Database=MyDb;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models

Use the model above:

    OnModelCreating(DapperSqlGenerator.Configure());

Available functions:

    using Dapper.SqlGenerator;

    string insertSql = connection.Sql().Insert<Product>();
    string insertAndReturnSql = connection.Sql().InsertReturn<Product>();
    string updateSql = connection.Sql().Update<Product>();
    string deleteSql = connection.Sql().Delete<Product>();
    string tableName = connection.Sql().Table<Order>();
    string commaSeparatedNonKeyColumns = connection.Sql().GetColumns<Order>(ColumnSelection.NonKeys | ColumnSelection.Computed);
    string keysAtQueryParams = connection.Sql().GetParams<Order>(ColumnSelection.Keys);
    string columnEqualParams = connection.Sql().GetColumnEqualParams<Order>(ColumnSelection.NonKeys);
    
    connection.Sql().HasColumnSet("unique_order", x => x.OrderId, x => x.ProductId);
    string upsertMergeSql = connection.Sql().Merge<Order>("unique_order");

# Dapper.SqlGenerator.Async Quick Start

SELECT and CRUD:

    using Dapper.SqlGenerator;
    using Dapper.SqlGenerator.Async;

    connection.Sql().HasColumnSet<Product>("id+kind+content", x => x.Id, x => x.Kind, x => x.Content);

    var all = await connection.SelectAsync<Product>();
    var allWithSomeColumns = await connection.SelectAsync<Product>("id+kind+content");
    var filtered = await connection.SelectWhereAsync<Product>("WHERE Kind > 5");
    var firstWithSomeColumns = await connection.SelectFirstAsync<Product>(new { Id = 1 }, "id+kind+content");
    var fisrtOrNull = connection.SelectFirstOrDefaultAsync<Product>(new { Id = -1 });
    var single =  = await connection.SelectSingleAsync<Product>(new { Id = 1 });
    var singleOrNull =  = await connection.SelectSingleOrDefaultAsync<Product>(new { Id = 1 });

    var rows1 = await connection.InsertAsync(new Product { Kind = 4, Content = "Square" });
    var inserted1 = await connection.InsertReturnAsync(new Product { Kind = 7, Content = "Triangle" });
    var rows2 = await connection.UpdateAsync(new Product { Id = 1, Kind = 66, Content = "Modified" });
    var deleted = await connection.DeleteAsync(new Product { Id = 2 });

    connection.Sql().HasColumnSet<Product>("content", x => x.Content);
    var insertedOrUpdated = await connection.MergeAsync(new Product { Kind = 7, Content = "Triangle", Value = 123 }, "content", "id+kind+content");

Database migration from scripts embedded as resources in MigrationResources namespace, for Sqlite:

    using Dapper.SqlGenerator.Async.Migration;
    
    var connectionString = "Data Source=:memory:";
    await using var connection = new SQLiteConnection(connectionString);
    connection.Open();

    var namespaceName = Assembly.GetExecutingAssembly().GetName().Name + ".MigrationResources";
    var noMigrationsApplied = await connection.InitDatabase(Assembly.GetExecutingAssembly(), namespaceName);
