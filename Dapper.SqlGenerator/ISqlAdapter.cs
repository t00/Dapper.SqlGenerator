using System;
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
        /// <param name="columnSet"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        string Insert<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, bool insertKeys, string columnSet);

        /// <summary>
        /// Inserts a row and returns keys 
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="table"></param>
        /// <param name="insertKeys"></param>
        /// <param name="columnSet"></param>
        string InsertReturn<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, bool insertKeys, string columnSet);

        /// <summary>
        /// Updates a row
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="table"></param>
        /// <param name="columnSet"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        string Update<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, string columnSet);

        /// <summary>
        /// Deletes a row
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="table"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        string Delete<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table);

        /// <summary>
        /// Prepares a merge (upsert - update or insert) operation on the entity 
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="table"></param>
        /// <param name="mergeSet"></param>
        /// <param name="insertKeys"></param>
        /// <param name="columnSet"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        string Merge<TEntity>(ModelBuilder modelBuilder, EntityTypeBuilder<TEntity> table, string mergeSet, bool insertKeys, string columnSet);
    }
}