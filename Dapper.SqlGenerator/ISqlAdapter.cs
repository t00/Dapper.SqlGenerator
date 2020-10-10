using System.Text;

namespace Dapper.SqlGenerator
{
    public interface ISqlAdapter : IBaseSqlAdapter
    {
        /// <summary>
        /// Inserts a row and returns keys 
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="table"></param>
        /// <param name="insertKeys"></param>
        string InsertReturn<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, bool insertKeys);

        string Insert<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, bool insertKeys);
    }
}