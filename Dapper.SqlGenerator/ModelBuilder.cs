using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Dapper.SqlGenerator
{
    public partial class ModelBuilder : ISql
    {
        private readonly ConcurrentDictionary<Type, EntityTypeBuilder> tableDict;
        private readonly ConcurrentDictionary<(string key, Type type, ColumnSelection selection), IList<PropertyBuilder>> columnCache = new ConcurrentDictionary<(string key, Type, ColumnSelection), IList<PropertyBuilder>>();
        private readonly ConcurrentDictionary<(string key, Type type), string> queryCache = new ConcurrentDictionary<(string key, Type type), string>();

        public ModelBuilder()
        {
            Shared = new EntityTypeBuilder();
            tableDict = new ConcurrentDictionary<Type, EntityTypeBuilder>();
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

        public ModelBuilder HasDefaultKeyColumn(string propertyName, Action<PropertyBuilder> options = null, Type adapter = null)
        {
            var property = Shared.Property(propertyName, adapter);
            property.IsKey = true;
            options?.Invoke(property);
            return this;
        }

        public IList<PropertyBuilder> GetProperties<TEntity>(ColumnSelection selection = ColumnSelection.Select)
        {
            return columnCache.GetOrAdd((nameof(GetProperties), typeof(TEntity), selection), key => SelectColumns<TEntity>(selection).ToList());
        }

        public string GetColumns<TEntity>(ColumnSelection selection)
        {
            return string.Join(",", GetProperties<TEntity>(selection).Select(x => Adapter.GetColumn(x, selection)).Where(x => x != null));
        }
        
        public string GetParams<TEntity>(ColumnSelection selection)
        {
            return string.Join(",", GetProperties<TEntity>(selection).Select(x => Adapter.GetParam(x, selection)).Where(x => x != null));
        }
        
        public string GetColumnEqualParams<TEntity>(ColumnSelection selection)
        {
            return string.Join(",", GetProperties<TEntity>(selection).Select(x => Adapter.GetColumnEqualParam(x, selection)).Where(x => x != null));
        }
        
        public string Insert<TEntity>(bool insertKeys = false)
        {
            return CacheQuery<TEntity>($"{nameof(Insert)}_{insertKeys}", () =>
            {
                var table = EnsureEntity<TEntity>();
                return Adapter.Insert(this, table, insertKeys);
            });
        }

        public string InsertReturn<TEntity>(bool insertKeys = false)
        {
            return CacheQuery<TEntity>($"{nameof(InsertReturn)}_{insertKeys}", () =>
            {
                var table = EnsureEntity<TEntity>();
                return Adapter.InsertReturn(this, table, insertKeys);
            });
        }

        public string Update<TEntity>()
        {
            return CacheQuery<TEntity>(nameof(Update), () =>
            {
                var table = EnsureEntity<TEntity>();
                return Adapter.Update(this, table);
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

        private string CacheQuery<TEntity>(string key, Func<string> buildFunction)
        {
            return queryCache.GetOrAdd((key, typeof(TEntity)), (_) => buildFunction());
        }
        
        private EntityTypeBuilder<TEntity> EnsureEntity<TEntity>()
        {
            return (EntityTypeBuilder<TEntity>) tableDict.GetOrAdd(typeof(TEntity), _ => new EntityTypeBuilder<TEntity>());
        }

        private IEnumerable<PropertyBuilder> SelectColumns<TEntity>(ColumnSelection selection)
        {
            var entity = EnsureEntity<TEntity>();
            var properties = entity.GetProperties(this);
            return properties.Where(x => Adapter.IsSelected(x, selection));
        }
   }
}