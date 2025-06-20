using System.Collections.Generic;

namespace AdoNetHelper.Abstract
{
    /// <summary>
    /// Basic CRUD operations interface.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    /// <typeparam name="K">Type of the entity key.</typeparam>
    public interface IDbOperations<T, K>
    {
        /// <summary>
        /// Inserts an entity.
        /// </summary>
        /// <returns>Affected row count.</returns>
        int Insert();

        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <returns>Affected row count.</returns>
        int Update();

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <returns>Affected row count.</returns>
        int Delete();

        /// <summary>
        /// Retrieves all entities.
        /// </summary>
        /// <returns>List of entities.</returns>
        List<T> SelectAll();

        /// <summary>
        /// Retrieves an entity by its key value.
        /// </summary>
        /// <param name="id">Identifier value.</param>
        /// <returns>Entity instance.</returns>
        T SelectById(K id);
    }
}
