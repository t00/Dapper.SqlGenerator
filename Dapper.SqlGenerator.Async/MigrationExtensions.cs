using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dapper.SqlGenerator.Async.Migration;

namespace Dapper.SqlGenerator.Async
{
    public static class ConnectionExtensions
    {
        private static readonly SemaphoreSlim InitLock = new SemaphoreSlim(1);
        
        private static readonly ConcurrentBag<(Type, string)> Applied = new ConcurrentBag<(Type, string)>();
        
        public static Task InitDatabase(this IDbConnection connection, string scriptsDirectory, SimpleMigrationOptions options = null)
        {
            return InitDatabase<SimpleMigration>(connection, scriptsDirectory, options);
        }

        public static async Task InitDatabase<TMigration>(this IDbConnection connection, string scriptsDirectory, MigrationOptions<TMigration> options = null)
            where TMigration: class, IMigration, new()
        {
            if (IsApplied(connection))
            {
                return;
            }
            
            await InitLock.WaitAsync();
            try
            {
                if (IsApplied(connection))
                {
                    return;
                }

                // Get all script files, scripts directory can be changed based on the database provider, i.e. db.GetType()
                // Scripts should be named yyyy-mm-dd to guarantee correct execution order
                // this is a blocking call in an async method but called only once per process
                var scripts = Directory.GetFiles(scriptsDirectory)
                    .Select(fileName => new MigrationScript
                    {
                        Name = Path.GetFileNameWithoutExtension(fileName).ToLower(),
                        Extension = Path.GetExtension(fileName).ToLower(),
                        GetContents = () => File.ReadAllTextAsync(fileName)
                    });
            
                await InitDatabase(connection, scripts, options);
                SetApplied(connection);
            }
            finally
            {
                InitLock.Release();
            }
        }

        public static Task InitDatabase(this IDbConnection connection, Assembly assembly, SimpleMigrationOptions options = null)
        {
            return InitDatabase<SimpleMigration>(connection, assembly, options);
        }

        public static async Task InitDatabase<TMigration>(this IDbConnection connection, Assembly assembly, MigrationOptions<TMigration> options = null)
            where TMigration: class, IMigration, new()
        {
            if (IsApplied(connection))
            {
                return;
            }
            
            await InitLock.WaitAsync();
            try
            {
                if (IsApplied(connection))
                {
                    return;
                }

                var scripts = assembly.GetManifestResourceNames().Select(resourceName => new MigrationScript
                {
                    Name = Path.GetFileName(resourceName),
                    Extension = Path.GetExtension(resourceName),
                    GetContents = async () =>
                    {
                        await using var stream = assembly.GetManifestResourceStream(resourceName);
                        if (stream == null)
                        {
                            throw new InvalidOperationException($"Stream not accessible: {resourceName}");
                        }

                        using var reader = new StreamReader(stream);
                        return await reader.ReadToEndAsync();
                    }
                });
            
                await InitDatabase(connection, scripts, options);
                SetApplied(connection);
            }
            finally
            {
                InitLock.Release();
            }
        }

        private static bool IsApplied(IDbConnection connection)
        {
            return Applied.Contains((connection.GetType(), connection.ConnectionString));
        }

        private static void SetApplied(IDbConnection connection)
        {
            Applied.Add((connection.GetType(), connection.ConnectionString));
        }

        private static async Task InitDatabase<TMigration>(this IDbConnection connection, IEnumerable<MigrationScript> scripts, MigrationOptions<TMigration> options = null)
            where TMigration: class, IMigration, new()
        {
            options ??= new MigrationOptions<TMigration>();
            var adapter = connection.Sql().Adapter;
            var migrationsSql = options.CheckHasMigrationsSql?.Invoke(adapter);
            var appliedMigrations = await GetAppliedMigrations<TMigration>(connection, migrationsSql);

            var missing = scripts
                .Where(x => !appliedMigrations.Contains(x.Name) && (
                    string.Equals(x.Extension, options.DefaultExtension, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(x.Extension, options.GetExtension?.Invoke(connection), StringComparison.OrdinalIgnoreCase)))
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Extension == options.DefaultExtension);

            foreach (var script in missing)
            {
                await ApplyMigration(connection, script, adapter, options);
            }
        }

        private static async Task<ICollection<string>> GetAppliedMigrations<TMigration>(IDbConnection connection, string migrationsSql)
            where TMigration : IMigration
        {
            var count = migrationsSql == null ? 0 : await connection.ExecuteScalarAsync<int>(migrationsSql, new { table = "migrations" });
            var migrations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (count > 0)
            {
                connection.Sql().HasColumnSet<TMigration>("migration_name", x => x.Name);
                var sql = connection.Sql().Select<TMigration>("migration_name");
                migrations = (await connection.QueryAsync<string>(sql)).ToHashSet(StringComparer.OrdinalIgnoreCase);
            }

            return migrations;
        }
        
        private static async Task ApplyMigration<TMigration>(IDbConnection connection, MigrationScript migrationScript, ISqlAdapter adapter, MigrationOptions<TMigration> options)
            where TMigration : class, IMigration, new()
        {
            var migration = new TMigration { Name = migrationScript.Name, Date = DateTime.UtcNow };
            var useTransaction = options.UseTransaction?.Invoke(migration, adapter) ?? true;
            var sql = await migrationScript.GetContents();
            var trans = useTransaction ? connection.BeginTransaction() : null;
            try
            {
                options.BeforeAction?.Invoke(migration, adapter);
                await connection.ExecuteAsync(sql);
                connection.Sql().HasColumnSet<TMigration>("migration_name_date", x => x.Name, x => x.Date);
                var migrationSql = connection.Sql().Insert<TMigration>("migration_name_date");
                await connection.ExecuteAsync(migrationSql, migration);
                options.AfterAction?.Invoke(migration, adapter);
                trans?.Commit();
            }
            catch (Exception)
            {
                trans?.Rollback();
                throw;
            }
        }
    }
}