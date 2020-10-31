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
            return InitDatabase<Migration.Migration>(connection, scriptsDirectory, options);
        }

        public static async Task<int> InitDatabase<TMigration>(this IDbConnection connection, string scriptsDirectory, MigrationOptions<TMigration> options = null)
            where TMigration: class, IMigration, new()
        {
            if (IsApplied(connection))
            {
                return 0;
            }
            
            await InitLock.WaitAsync();
            try
            {
                if (IsApplied(connection))
                {
                    return 0;
                }

                // Get all script files, scripts directory can be changed based on the database provider, i.e. db.GetType()
                // Scripts should be named yyyy-mm-dd to guarantee correct execution order
                // this is a blocking call in an async method but called only once per process
                var scripts = Directory.GetFiles(scriptsDirectory)
                    .Select(fileName => new MigrationScript
                    {
                        Name = Path.GetFileNameWithoutExtension(fileName).ToLower(),
                        Extension = Path.GetExtension(fileName).TrimStart('.').ToLower(),
                        GetContents = () => File.ReadAllTextAsync(fileName)
                    });
            
                var count = await InitDatabase(connection, scripts, options);
                SetApplied(connection);
                return count;
            }
            finally
            {
                InitLock.Release();
            }
        }

        public static Task<int> InitDatabase(this IDbConnection connection, Assembly assembly, string resourceNamespace, SimpleMigrationOptions options = null)
        {
            return InitDatabase<Migration.Migration>(connection, assembly, resourceNamespace, options);
        }

        public static async Task<int> InitDatabase<TMigration>(this IDbConnection connection, Assembly assembly, string resourceNamespace = null, MigrationOptions<TMigration> options = null)
            where TMigration: class, IMigration, new()
        {
            if (IsApplied(connection) && options?.ForceApplyMissing != true)
            {
                return 0;
            }
            
            await InitLock.WaitAsync();
            try
            {
                if (IsApplied(connection) && options?.ForceApplyMissing != true)
                {
                    return 0;
                }

                resourceNamespace ??= string.Empty;
                var scripts = assembly.GetManifestResourceNames()
                    .Where(x => x.StartsWith(resourceNamespace))
                    .Select(resourceName => new MigrationScript
                {
                    Name = Path.GetFileNameWithoutExtension(GetResourceName(resourceName)).ToLower(),
                    Extension = Path.GetExtension(GetResourceName(resourceName)).ToLower().TrimStart('.'),
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
            
                var count = await InitDatabase(connection, scripts, options);
                SetApplied(connection);
                return count;
            }
            finally
            {
                InitLock.Release();
            }
            
            string GetResourceName(string name)
            {
                name = name.Substring(resourceNamespace.Length);
                return name.TrimStart('.');
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

        private static async Task<int> InitDatabase<TMigration>(this IDbConnection connection, IEnumerable<MigrationScript> scripts, MigrationOptions<TMigration> options = null)
            where TMigration: class, IMigration, new()
        {
            options ??= new MigrationOptions<TMigration>();
            var generator = connection.Sql();
            var appliedMigrations = await GetAppliedMigrations<TMigration>(connection, generator);

            var missing = scripts
                .Where(x => !appliedMigrations.Contains(x.Name) && (
                    string.Equals(x.Extension, options.DefaultExtension, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(x.Extension, options.GetExtension?.Invoke(connection), StringComparison.OrdinalIgnoreCase)))
                .GroupBy(x => x.Name)
                .ToDictionary(x => x.Key, 
                    x => x.OrderBy(e => e.Extension == options.DefaultExtension).ToList());

            foreach (var script in missing.OrderBy(x => x.Key))
            {
                await ApplyMigration(connection, script, generator.Adapter, options);
            }

            return missing.Count;
        }

        private static async Task<ICollection<string>> GetAppliedMigrations<TMigration>(IDbConnection connection, ISql generator)
            where TMigration : IMigration
        {
            var migrationsSql = generator.Adapter.TableExists();
            var tableName = generator.Table<TMigration>(false);
            var count = migrationsSql == null ? 0 : await connection.ExecuteScalarAsync<int>(migrationsSql, new { table = tableName });
            var migrations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (count > 0)
            {
                connection.Sql().HasColumnSet<TMigration>("migration_name", x => x.Name);
                var sql = connection.Sql().Select<TMigration>("migration_name");
                migrations = (await connection.QueryAsync<string>(sql)).ToHashSet(StringComparer.OrdinalIgnoreCase);
            }

            return migrations;
        }
        
        private static async Task ApplyMigration<TMigration>(IDbConnection connection, KeyValuePair<string, List<MigrationScript>> migrationScripts, ISqlAdapter adapter, MigrationOptions<TMigration> options)
            where TMigration : class, IMigration, new()
        {
            var migration = new TMigration { Name = migrationScripts.Key, Date = DateTime.UtcNow };
            var useTransaction = options.UseTransaction?.Invoke(migration, adapter) ?? true;
            var trans = useTransaction ? connection.BeginTransaction() : null;
            try
            {
                options.BeforeAction?.Invoke(migration, adapter);
                foreach (var migrationScript in migrationScripts.Value)
                {
                    var sql = await migrationScript.GetContents();
                    await connection.ExecuteAsync(sql);
                }
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