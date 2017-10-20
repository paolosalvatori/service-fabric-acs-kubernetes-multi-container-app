#region Using Directives
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApi.Models;
#endregion

namespace TodoApi.Services
{
    /// <summary>
    /// This is an abstract base class for the repository service.
    /// </summary>
    public abstract class RepositoryService<T> : IRepositoryService<T> where T : Entity, new()
    {
        #region Public Virtual Methods
        /// <summary>
        /// Reads a Document from the Azure DocumentDB database service as an asynchronous operation.
        /// </summary>
        /// <param name="id">Document id</param>
        /// <returns>A Task that wraps the T entity retrieved.</returns>
        public virtual Task<T> GetByIdAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Reads all the documents in the document collection.
        /// </summary>
        /// <returns>A Task that wraps a collection of T entities.</returns>
        public virtual Task<IEnumerable<T>> GetAllAsync()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Creates a Document as an asychronous operation in the Azure DocumentDB database service.
        /// </summary>
        /// <param name="entity">The entity to create</param>
        /// <returns>A task.</returns>
        public virtual Task CreateAsync(T entity)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Updates a Document as an asychronous operation in the Azure DocumentDB database service.
        /// </summary>
        /// <param name="entity">The entity to update</param>
        /// <returns>A task.</returns>
        public virtual Task UpdateAsync(T entity)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Deletes a Document from the Azure DocumentDB database service as an asynchronous operation.
        /// </summary>
        /// <param name="id">Document id</param>
        /// <returns>A Task that wraps the T entity retrieved.</returns>
        public virtual Task DeleteByIdAsync(string id)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
