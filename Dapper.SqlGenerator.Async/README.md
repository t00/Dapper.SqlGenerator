# Dapper.SqlGenerator.Async

Note: Documentation is not complete, check unit tests for more examples.

## Usage - Select

All typed Dapper Query variants are available, some examples for SELECT queries:

    using Dapper.SqlGenerator.Async;

    connection.Sql().HasColumnSet<Product>("id+kind+content", x => x.Id, x => x.Kind, x => x.Content);

    var all = await connection.SelectAsync<Product>();
    var allWithSomeColumns = await connection.SelectAsync<Product>("id+kind+content");
    var filtered = await connection.SelectWhereAsync<Product>("WHERE Kind > 5");
    var firstWithSomeColumns = await connection.SelectFirstAsync<Product>(new { Id = 1 }, "id+kind+content");
    var fisrtOrNull = connection.SelectFirstOrDefaultAsync<Product>(new { Id = -1 });
    var single =  = await connection.SelectSingleAsync<Product>(new { Id = 1 });
    var singleOrNull =  = await connection.SelectSingleOrDefaultAsync<Product>(new { Id = 1 });
  
## Usage - INSERT, UPDATE, DELETE, MERGE
    
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
    var affectedInsertOrUpdateRows = await connection.MergeAsync(new Product { Kind = 8, Content = "Triangle", Value = 123 }, "content", "id+kind+content+value");

## Usage - Migrations

Resources (or files) containing SQL scripts must have an extension which is either "sql" (can be changed using options) or the name of ISqlAdapter class, lowercase i.e. "npgsqlconnection". Resource name should be prepared for correct ordering when applying migrations.

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
