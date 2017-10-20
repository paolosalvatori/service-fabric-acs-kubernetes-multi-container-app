#region Using Directives
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Models;
#endregion

namespace TodoApi.Services
{
    /// <summary>
    ///  This class is used to read, write, delete and update data from Cosmos DB using Document DB API. 
    /// </summary>
    public class CosmosDbRepositoryService<T> : IRepositoryService<T> where T : Entity, new()
    {
        #region Private Instance Fields
        private readonly RepositoryServiceOptions _repositoryServiceOptions;
        private readonly DocumentClient _documentClient;
        private readonly ILogger<CosmosDbRepositoryService<T>> _logger;
        #endregion

        #region Public Constructor
        /// <summary>
        /// Creates a new instance of the ServiceBusNotificationService class
        /// </summary>
        public CosmosDbRepositoryService(IOptions<RepositoryServiceOptions> options,
                                         ILogger<CosmosDbRepositoryService<T>> logger)
        {
            if (options?.Value == null)
            {
                throw new ArgumentNullException(nameof(options), "No configuration is defined for the repository service in the appsettings.json.");
            }

            if (options.Value.CosmosDb == null)
            {
                throw new ArgumentNullException(nameof(options), "No CosmosDb element is defined in the configuration for the notification service in the appsettings.json.");
            }

            if (string.IsNullOrWhiteSpace(options.Value.CosmosDb.EndpointUri))
            {
                throw new ArgumentNullException(nameof(options), "No endpoint uri is defined in the configuration of the Cosmos DB notification service in the appsettings.json.");
            }

            if (string.IsNullOrWhiteSpace(options.Value.CosmosDb.PrimaryKey))
            {
                throw new ArgumentNullException(nameof(options), "No primary key is defined in the configuration of the Cosmos DB notification service in the appsettings.json.");
            }

            if (string.IsNullOrWhiteSpace(options.Value.CosmosDb.DatabaseName))
            {
                throw new ArgumentNullException(nameof(options), "No database name is defined in the configuration of the Cosmos DB notification service in the appsettings.json.");
            }

            if (string.IsNullOrWhiteSpace(options.Value.CosmosDb.CollectionName))
            {
                throw new ArgumentNullException(nameof(options), "No collection name is defined in the configuration of the Cosmos DB notification service in the appsettings.json.");
            }

            _repositoryServiceOptions = options.Value;
            _logger = logger;

            _documentClient = new DocumentClient(new Uri(_repositoryServiceOptions.CosmosDb.EndpointUri),
                                                 options.Value.CosmosDb.PrimaryKey,
                                                 new ConnectionPolicy
                                                 {
                                                     //ConnectionMode = ConnectionMode.Direct, // Not supported when running the ASP.NET Core app in a Docker container
                                                     ConnectionProtocol = Protocol.Tcp,
                                                     RequestTimeout = TimeSpan.FromMinutes(5)
                                                 });
            CreateDatabaseAndDocumentCollectionIfNotExistsAsync().Wait();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Reads a Document from the Azure DocumentDB database service as an asynchronous operation.
        /// </summary>
        /// <param name="id">Document id</param>
        /// <returns>A Task that wraps the T entity retrieved.</returns>
        public async Task<T> GetByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new ArgumentNullException(nameof(id), "The id cannot null or empty.");
                }
                var documentUri = UriFactory.CreateDocumentUri(_repositoryServiceOptions.CosmosDb.DatabaseName,
                                                               _repositoryServiceOptions.CosmosDb.CollectionName,
                                                               id);
                var response = await _documentClient.ReadDocumentAsync<T>(documentUri);
                return response.Document;
            }
            catch (DocumentClientException e)
            {
                var baseException = e.GetBaseException();
                _logger.LogError(LoggingEvents.GetItem,
                                 e,
                                 $"An error occurred: StatusCode=[{e.StatusCode}] Message=[{e.Message}] BaseException=[{baseException?.Message ?? "NULL"}]");
                throw;
            }
            catch (Exception e)
            {
                var baseException = e.GetBaseException();
                _logger.LogError(LoggingEvents.GetItem,
                                e,
                                $"An error occurred: Message=[{e.Message}] BaseException=[{baseException?.Message ?? "NULL"}]");
                throw;
            }
        }

        /// <summary>
        /// Reads all the documents in the document collection.
        /// </summary>
        /// <returns>A Task that wraps a collection of T entities.</returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                var documentCollectionUri = UriFactory.CreateDocumentCollectionUri(_repositoryServiceOptions.CosmosDb.DatabaseName,
                                                                                   _repositoryServiceOptions.CosmosDb.CollectionName);
                var queryOptions = new FeedOptions { MaxItemCount = -1 };
                var query = _documentClient.CreateDocumentQuery<T>(documentCollectionUri, queryOptions);
                return await Task.FromResult<IEnumerable<T>>(query.ToList());
            }
            catch (DocumentClientException e)
            {
                var baseException = e.GetBaseException();
                _logger.LogError(LoggingEvents.ListItems,
                                 e,
                                 $"An error occurred: StatusCode=[{e.StatusCode}] Message=[{e.Message}] BaseException=[{baseException?.Message ?? "NULL"}]");
                throw;
            }
            catch (Exception e)
            {
                var baseException = e.GetBaseException();
                _logger.LogError(LoggingEvents.ListItems,
                                e,
                                $"An error occurred: Message=[{e.Message}] BaseException=[{baseException?.Message ?? "NULL"}]");
                throw;
            }
        }

        /// <summary>
        /// Creates a Document as an asychronous operation in the Azure DocumentDB database service.
        /// </summary>
        /// <param name="entity">The entity to create</param>
        /// <returns>A task.</returns>
        public async Task CreateAsync(T entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity), "The entity cannot null.");
                }

                var documentCollectionUri = UriFactory.CreateDocumentCollectionUri(_repositoryServiceOptions.CosmosDb.DatabaseName,
                                                                                   _repositoryServiceOptions.CosmosDb.CollectionName);
                var response = await _documentClient.CreateDocumentAsync(documentCollectionUri, entity);
            }
            catch (DocumentClientException e)
            {
                var baseException = e.GetBaseException();
                _logger.LogError(LoggingEvents.GetItem,
                                 e,
                                 $"An error occurred: StatusCode=[{e.StatusCode}] Message=[{e.Message}] BaseException=[{baseException?.Message ?? "NULL"}]");
                throw;
            }
            catch (Exception e)
            {
                var baseException = e.GetBaseException();
                _logger.LogError(LoggingEvents.GetItem,
                                e,
                                $"An error occurred: Message=[{e.Message}] BaseException=[{baseException?.Message ?? "NULL"}]");
                throw;
            }
        }

        /// <summary>
        /// Updates a Document as an asychronous operation in the Azure DocumentDB database service.
        /// </summary>
        /// <param name="entity">The entity to update</param>
        /// <returns>A task.</returns>
        public async Task UpdateAsync(T entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity), "The entity cannot null.");
                }

                var documentCollectionUri = UriFactory.CreateDocumentCollectionUri(_repositoryServiceOptions.CosmosDb.DatabaseName,
                                                                                   _repositoryServiceOptions.CosmosDb.CollectionName);
                var response = await _documentClient.UpsertDocumentAsync(documentCollectionUri, entity);
            }
            catch (DocumentClientException e)
            {
                var baseException = e.GetBaseException();
                _logger.LogError(LoggingEvents.GetItem,
                                 e,
                                 $"An error occurred: StatusCode=[{e.StatusCode}] Message=[{e.Message}] BaseException=[{baseException?.Message ?? "NULL"}]");
                throw;
            }
            catch (Exception e)
            {
                var baseException = e.GetBaseException();
                _logger.LogError(LoggingEvents.GetItem,
                                e,
                                $"An error occurred: Message=[{e.Message}] BaseException=[{baseException?.Message ?? "NULL"}]");
                throw;
            }
        }

        /// <summary>
        /// Deletes a Document from the Azure DocumentDB database service as an asynchronous operation.
        /// </summary>
        /// <param name="id">Document id</param>
        /// <returns>A Task that wraps the T entity retrieved.</returns>
        public async Task DeleteByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new ArgumentNullException(nameof(id), "The id cannot null or empty.");
                }
                var documentUri = UriFactory.CreateDocumentUri(_repositoryServiceOptions.CosmosDb.DatabaseName,
                                                               _repositoryServiceOptions.CosmosDb.CollectionName,
                                                               id);
                var response = await _documentClient.DeleteDocumentAsync(documentUri);
            }
            catch (DocumentClientException e)
            {
                var baseException = e.GetBaseException();
                _logger.LogError(LoggingEvents.GetItem,
                                 e,
                                 $"An error occurred: StatusCode=[{e.StatusCode}] Message=[{e.Message}] BaseException=[{baseException?.Message ?? "NULL"}]");
                throw;
            }
            catch (Exception e)
            {
                var baseException = e.GetBaseException();
                _logger.LogError(LoggingEvents.GetItem,
                                e,
                                $"An error occurred: Message=[{e.Message}] BaseException=[{baseException?.Message ?? "NULL"}]");
                throw;
            }
        }
        #endregion

        #region Private Instance Fields
        private async Task CreateDatabaseAndDocumentCollectionIfNotExistsAsync()
        {
            await _documentClient.CreateDatabaseIfNotExistsAsync(new Database
            {
                Id = _repositoryServiceOptions.CosmosDb.DatabaseName
            });
            await _documentClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(_repositoryServiceOptions.CosmosDb.DatabaseName),
            new DocumentCollection
            {
                Id = _repositoryServiceOptions.CosmosDb.CollectionName
            });
        }
        #endregion
    }
}
