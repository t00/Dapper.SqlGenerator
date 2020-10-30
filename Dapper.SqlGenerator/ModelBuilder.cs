using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Dapper.SqlGenerator
{
    public class ModelBuilder : ISql
    {
        private readonly ConcurrentDictionary<Type, EntityTypeBuilder> tableDict;
        private readonly ConcurrentDictionary<(string key, Type type, ColumnSelection selection, string columnSet), IList<PropertyBuilder>> columnCache = new ConcurrentDictionary<(string key, Type, ColumnSelection, string), IList<PropertyBuilder>>();
        private readonly ConcurrentDictionary<(string key, Type type), string> queryCache = new ConcurrentDictionary<(string key, Type type), string>();

        public ModelBuilder()
        {
            Shared = new EntityTypeBuilder(null);
            tableDict = new ConcurrentDictionary<Type, EntityTypeBuilder>();
            HasDefaultKeyColumn("Id");
        }

        public ModelBuilder(ModelBuilder source)
        {
            Shared = source.Shared;
            tableDict = source.tableDict;
        }

        public EntityTypeBuilder Shared { get; }

        public ISqlAdapter Adapter { get; set; }

        public ModelBuilder Entity<TEntity>(Action<EntityTypeBuilder<TEntity>> action = null)
        {
            var entityBuilder = EnsureEntity<TEntity>();
            action?.Invoke(entityBuilder);
            return this;
        }

        public string Table<TEntity>()
        {
            return Adapter.GetTableName(EnsureEntity<TEntity>());
        }
        
        public ISql HasColumnSet<TEntity>(string name, Type adapter, params string[] columns)
        {
            EnsureEntity<TEntity>().HasColumnSet(name, adapter, columns);
            return this;
        }

        public ISql HasColumnSet<TEntity>(string name, params string[] columns)
        {
            EnsureEntity<TEntity>().HasColumnSet(name, columns);
            return this;
        }

        public ISql HasColumnSet<TEntity>(string name, Type adapter, params Expression<Func<TEntity, object>>[] columns)
        {
            EnsureEntity<TEntity>().HasColumnSet(name, adapter, columns);
            return this;
        }

        public ISql HasColumnSet<TEntity>(string name, params Expression<Func<TEntity, object>>[] columns)
        {
            EnsureEntity<TEntity>().HasColumnSet(name, columns);
            return this;
        }

        public ModelBuilder HasDefaultKeyColumn(string propertyName, Action<PropertyBuilder> options = null, Type adapter = null)
        {
            var property = Shared.Property(propertyName, adapter);
            property.IsKey = true;
            options?.Invoke(property);
            return this;
        }

        public IList<PropertyBuilder> GetProperties<TEntity>(ColumnSelection selection = ColumnSelection.Select, string columnSet = null)
        {
            return columnCache.GetOrAdd((nameof(GetProperties), typeof(TEntity), selection, columnSet), key => SelectColumns<TEntity>(selection, columnSet).ToList());
        }

        IEnumerable<IProperty> ISql.GetProperties<TEntity>(string columnSet, ColumnSelection selection)
        {
            return GetProperties<TEntity>(selection, columnSet);
        }

        public string GetColumns<TEntity>(string columnSet = null, ColumnSelection selection = ColumnSelection.Select, string alias = null, string separator = ",")
        {
            return string.Join(separator, GetProperties<TEntity>(selection, columnSet).Select(x => Adapter.GetColumn(x, selection, alias)).Where(x => x != null));
        }

        public string GetParams<TEntity>(string columnSet = null, ColumnSelection selection = ColumnSelection.Select, string separator = ",")
        {
            return string.Join(separator, GetProperties<TEntity>(selection, columnSet).Select(x => Adapter.GetParam(x, selection)).Where(x => x != null));
        }
        
        public string GetColumnEqualParams<TEntity>(string columnSet = null, ColumnSelection selection = ColumnSelection.Select, string alias = null, string separator = ",")
        {
            return string.Join(separator, GetProperties<TEntity>(selection, columnSet).Select(x => Adapter.GetColumnEqualParam(x, selection, alias)).Where(x => x != null));
        }

        public string Select<TEntity>(string columnSet = null, string alias = null)
        {
            return CacheQuery<TEntity>($"{nameof(Select)}_{columnSet}_{alias}", () =>
            {
                var table = EnsureEntity<TEntity>();
                return Adapter.Select(this, table, columnSet, ColumnSelection.Select, null, ColumnSelection.None, alias);
            });
        }

        public string SelectSingle<TEntity>(string columnSet = null, string alias = null)
        {
            return CacheQuery<TEntity>($"{nameof(SelectSingle)}_{columnSet}_{alias}", () =>
            {
                var table = EnsureEntity<TEntity>();
                return Adapter.Select(this, table, columnSet, ColumnSelection.Select, null, ColumnSelection.Keys, alias);
            });
        }

        public string SelectWhere<TEntity>(string whereSet, string columnSet = null, string alias = null)
        {
            return CacheQuery<TEntity>($"{nameof(SelectWhere)}_{whereSet}_{columnSet}_{alias}", () =>
            {
                var table = EnsureEntity<TEntity>();
                return Adapter.Select(this, table, columnSet, ColumnSelection.Select, whereSet, ColumnSelection.Keys | ColumnSelection.NonKeys, alias);
            });
        }

        public string Insert<TEntity>(string columnSet = null, bool insertKeys = false)
        {
            return CacheQuery<TEntity>($"{nameof(Insert)}_{insertKeys}_{columnSet}", () =>
            {
                var table = EnsureEntity<TEntity>();
                return Adapter.Insert(this, table, insertKeys, columnSet);
            });
        }

        public string InsertReturn<TEntity>(string columnSet = null, bool insertKeys = false)
        {
            return CacheQuery<TEntity>($"{nameof(InsertReturn)}_{insertKeys}_{columnSet}", () =>
            {
                var table = EnsureEntity<TEntity>();
                return Adapter.InsertReturn(this, table, insertKeys, columnSet);
            });
        }

        public string Update<TEntity>(string columnSet = null)
        {
            return CacheQuery<TEntity>($"{nameof(Update)}_{columnSet}", () =>
            {
                var table = EnsureEntity<TEntity>();
                return Adapter.Update(this, table, columnSet);
            });
        }

        public string Delete<TEntity>()
        {
            return CacheQuery<TEntity>(nameof(Delete), () =>
            {
                var table = EnsureEntity<TEntity>();
                return Adapter.Delete(this, table);
            });
        }

        public string Merge<TEntity>(string mergeSet, string columnSet = null, bool insertKeys = false)
        {
            return CacheQuery<TEntity>($"{nameof(Merge)}_{insertKeys}_{columnSet}", () =>
            {
                var table = EnsureEntity<TEntity>();
                return Adapter.Merge(this, table, mergeSet, insertKeys, columnSet);
            });
        }

        private string CacheQuery<TEntity>(string key, Func<string> buildFunction)
        {
            return queryCache.GetOrAdd((key, typeof(TEntity)), (_) => buildFunction());
        }
        
        private EntityTypeBuilder<TEntity> EnsureEntity<TEntity>()
        {
            return (EntityTypeBuilder<TEntity>) tableDict.GetOrAdd(typeof(TEntity), _ => new EntityTypeBuilder<TEntity>(Shared));
        }

        private IEnumerable<PropertyBuilder> SelectColumns<TEntity>(ColumnSelection selection, string columnSet)
        {
            var entity = EnsureEntity<TEntity>();
            var properties = entity
                .GetProperties(this)
                .Where(x => Adapter.IsSelected(x, selection));

            if (columnSet == null)
            {
                return properties;
            }

            if (!entity.ColumnSetsDict.TryGetValue(columnSet, out var setColumns))
            {
                throw new SqlGenerationException($"Column set not defined: ${columnSet}");
            }

            return properties.Where(x => setColumns.Any(s => s.Name == x.Name));
        }
    }
}