#region Using Directives
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApi.Models;
#endregion

namespace TodoApi.Services
{
    /// <summary>
    /// Interface implemented by notification services
    /// </summary>
    public interface IRepositoryService<T> where T : Entity, new()
    {
        #region Interface Methods
        /// <summary>
        /// Reads a Document from the Azure DocumentDB database service as an asynchronous operation.
        /// </summary>
        /// <param name="id">Document id</param>
        /// <returns>A Task that wraps the T entity retrieved.</returns>
        Task<T> GetByIdAsync(string id);

        /// <summary>
        /// Reads all the documents in the document collection.
        /// </summary>
        /// <returns>A Task that wraps a collection of T entities.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Creates a Document as an asychronous operation in the Azure DocumentDB database service.
        /// </summary>
        /// <param name="entity">The entity to create</param>
        /// <returns>A task.</returns>
        Task CreateAsync(T entity);

        /// <summary>
        /// Updates a Document as an asychronous operation in the Azure DocumentDB database service.
        /// </summary>
        /// <param name="entity">The entity to update</param>
        /// <returns>A task.</returns>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Deletes a Document from the Azure DocumentDB database service as an asynchronous operation.
        /// </summary>
        /// <param name="id">Document id</param>
        /// <returns>A Task that wraps the T entity retrieved.</returns>
        Task DeleteByIdAsync(string id);
        #endregion
    }
}
