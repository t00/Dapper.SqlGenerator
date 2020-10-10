using System.Text;

namespace Dapper.SqlGenerator
{
    public interface ISqlAdapter : IBaseSqlAdapter
    {
        /// <summary>
        /// Inserts a row
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="table"></param>
        /// <param name="insertKeys"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        string Insert<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, bool insertKeys);
        
        /// <summary>
        /// Inserts a row and returns keys 
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="table"></param>
        /// <param name="insertKeys"></param>
        string InsertReturn<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, bool insertKeys);

        /// <summary>
        /// Updates a row
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="table"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        string Update<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table);

        /// <summary>
        /// Deletes a row
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="table"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        string Delete<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table);
    }
}