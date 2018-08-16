using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Edison.Repositories.Config;
using Edison.Common.Managers;
using Edison.Common.Models;
using System.Net;

namespace Edison.Repositories
{
    public class CosmosDBRepository<T> : CosmosDBBaseRepository, ICosmosDBRepository<T> where T : class, IEntityModel
    {
        public CosmosDBRepository(string endpoint, string authkey, string databaseId, string collectionId, ILogger<CosmosDBBaseRepository> logger) :
            base(endpoint, authkey, databaseId, collectionId, logger)
        {
        }

        public CosmosDBRepository(IOptionsSnapshot<CosmosDBOptions> config, ILogger<CosmosDBBaseRepository> logger) :
            base(config.Get(typeof(T).FullName).Endpoint,
                 config.Get(typeof(T).FullName).AuthKey,
                 config.Get(typeof(T).FullName).Database,
                 config.Get(typeof(T).FullName).Collection,
                 logger) 
        {
        }

        public async Task<T> GetItemAsync(string id)
        {
            try
            {
                Document document = await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id));
                T result = (T)(dynamic)document;
                result.ETag = document.ETag;
                return result;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    _logger.LogError("GetItemAsync: DocumentClientException: " + e.Error.Message);
                    throw;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("GetItemAsync: " + e.Message);
                return null;
            }
        }

        public async Task<T> GetItemAsync(Expression<Func<T, bool>> where)
        {
            try
            {
                IDocumentQuery<T> query = _client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId),
                    new FeedOptions { MaxItemCount = -1 })
                    .Where(where).AsDocumentQuery();

                return (await query.ExecuteNextAsync<T>()).FirstOrDefault();
            }
            catch (DocumentClientException e)
            {
                _logger.LogError("GetItemAsync: DocumentClientException: " + e.Error.Message);
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError("GetItemAsync: " + e.Message);
                return null;
            }
        }

        public async Task<bool> IsItemExistsByIdAsync(string Id)
        {
            try
            {
                Document document = await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, Id));
                return true;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return false;
                }
                else
                {
                    _logger.LogError("IsItemExistsByIdAsync: DocumentClientException: " + e.Error.Message);
                    throw e;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("IsItemExistsByIdAsync: " + e.Message);
                throw e;
            }
        }

        public async Task<bool> IsItemExistsByNonIdAsync(Expression<Func<T, bool>> where)
        {
            try
            {
                IDocumentQuery<T> query = _client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId),
                    new FeedOptions { MaxItemCount = -1 })
                    .Where(where)
                    .AsDocumentQuery();

                List<T> results = new List<T>();
                while (query.HasMoreResults)
                {
                    results.AddRange(await query.ExecuteNextAsync<T>());
                }
                if (results.Count == 0)
                    return false;
                else
                    return true;
            }
            catch (DocumentClientException e)
            {
                _logger.LogError("IsItemExistsByNonIdAsync: DocumentClientException: " + e.Error.Message);
                throw e;
            }
            catch (Exception e)
            {
                _logger.LogError("IsItemExistsByNonIdAsync: " + e.Message);
                throw e;
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> where, Expression<Func<T, T>> select)
        {
            try
            {
                IDocumentQuery<T> query = _client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId),
                    new FeedOptions { MaxItemCount = -1 })
                    .Where(where)
                    .Select(select)
                    .AsDocumentQuery();

                List<T> results = new List<T>();
                while (query.HasMoreResults)
                {
                    results.AddRange(await query.ExecuteNextAsync<T>());
                }
                return results;
            }
            catch (DocumentClientException e)
            {
                _logger.LogError("GetItemsAsync: DocumentClientException: " + e.Error.Message);
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError("GetItemsAsync: " + e.Message);
                return null;
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> where)
        {
            try
            {
                IDocumentQuery<T> query = _client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId),
                    new FeedOptions { MaxItemCount = -1 })
                    .Where(where)
                    .AsDocumentQuery();

                List<T> results = new List<T>();
                while (query.HasMoreResults)
                {
                    results.AddRange(await query.ExecuteNextAsync<T>());
                }
                return results;
            }
            catch (DocumentClientException e)
            {
                _logger.LogError("GetItemsAsync: DocumentClientException: " + e.Error.Message);
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError("GetItemsAsync: " + e.Message);
                return null;
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsyncOrderByAscending(Expression<Func<T, bool>> where, Expression<Func<T, DateTime>> orderBy, int limit = 100)
        {
            try
            {
                IDocumentQuery<T> query = _client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId),
                    new FeedOptions { MaxItemCount = -1 })
                    .Where(where).OrderBy(orderBy).Take(limit)
                    .AsDocumentQuery();

                List<T> results = new List<T>();
                while (query.HasMoreResults)
                {
                    results.AddRange(await query.ExecuteNextAsync<T>());
                }
                return results;
            }
            catch (DocumentClientException e)
            {
                _logger.LogError("GetItemsAsync: DocumentClientException: " + e.Error.Message);
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError("GetItemsAsync: " + e.Message);
                return null;
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsyncOrderByDescending(Expression<Func<T, bool>> where, Expression<Func<T, DateTime>> orderBy, int limit = 100)
        {
            try
            {
                IDocumentQuery<T> query = _client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId),
                    new FeedOptions { MaxItemCount = -1 })
                    .Where(where).OrderByDescending(orderBy).Take(limit)
                    .AsDocumentQuery();

                List<T> results = new List<T>();
                while (query.HasMoreResults)
                {
                    results.AddRange(await query.ExecuteNextAsync<T>());
                }
                return results;
            }
            catch (DocumentClientException e)
            {
                _logger.LogError("GetItemsAsync: DocumentClientException: " + e.Error.Message);
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError("GetItemsAsync: " + e.Message);
                return null;
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync()
        {
            try
            {
                IDocumentQuery<T> query = _client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId),
                    new FeedOptions { MaxItemCount = -1 })
                    .AsDocumentQuery();

                List<T> results = new List<T>();
                while (query.HasMoreResults)
                {
                    results.AddRange(await query.ExecuteNextAsync<T>());
                }
                return results;
            }
            catch (DocumentClientException e)
            {
                _logger.LogError("GetItemsAsync error: DocumentClientException: " + e.Error.Message);
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError("GetItemsAsync error: " + e.Message);
                return null;
            }
        }

        public async Task<string> CreateItemAsync(T item)
        {
            try
            {
                await EnsureDatabaseAndCollectionExists();
                item.Date = DateTime.UtcNow;
                Document doc = await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId), item);
                return doc.Id;
            }
            catch (DocumentClientException e)
            {
                _logger.LogError("CreateItemAsync: DocumentClientException: " + e.Error.Message);
                return string.Empty;
            }
            catch (Exception e)
            {
                _logger.LogError("CreateItemAsync: " + e.Message);
                return string.Empty;
            }
        }

        public async Task<bool> UpdateItemAsync(string id, T item)
        {
            try
            {
                await EnsureDatabaseAndCollectionExists();
                item.Date = DateTime.UtcNow;

                var requestOptions = new RequestOptions()
                {
                    AccessCondition = new AccessCondition()
                    {
                        Type = AccessConditionType.IfMatch,
                        Condition = item.ETag
                    }
                };

                Document doc = await _client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id), item, requestOptions);
                return true;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                    Console.WriteLine($"UpdateItemAsync: Etag exception: {e.Error.Message}");
                else
                    _logger.LogError($"UpdateItemAsync: DocumentClientException: {e.Error.Message}");
                throw e;
            }
            catch (Exception e)
            {
                _logger.LogError($"UpdateItemAsync: {e.Message}");
                throw e;
            }
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            try
            {
                await EnsureDatabaseAndCollectionExists();
                await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id));
                return true;
            }
            catch (DocumentClientException e)
            {
                _logger.LogError("DeleteItemAsync: DocumentClientException: " + e.Error.Message);
                throw e;
            }
            catch (Exception e)
            {
                _logger.LogError("DeleteItemAsync: " + e.Message);
                throw e;
            }
        }

        public async Task<bool> DeleteItemsAsync(Expression<Func<T, bool>> where)
        {
            try
            {
                await EnsureDatabaseAndCollectionExists();
                var results = await GetItemsAsync(where);
                foreach(var result in results)
                    await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, result.Id));
                return true;
            }
            catch (DocumentClientException e)
            {
                _logger.LogError("DeleteItemAsync: DocumentClientException: " + e.Error.Message);
                throw e;
            }
            catch (Exception e)
            {
                _logger.LogError("DeleteItemAsync: " + e.Message);
                throw e;
            }
        }
    }
}
