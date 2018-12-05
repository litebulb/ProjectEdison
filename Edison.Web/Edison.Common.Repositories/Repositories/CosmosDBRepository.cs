using Edison.Common.Config;
using Edison.Common.Interfaces;
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
using System.Net;

namespace Edison.Common
{
    public class CosmosDBRepository<T> : CosmosDBBaseRepository, ICosmosDBRepository<T> where T : class, IEntityDAO
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

        public async Task<T> GetItemAsync(Guid id)
        {
            return await GetItemAsync(id.ToString());
        }

        public async Task<T> GetItemAsync(string id)
        {
            try
            {
                Document document = await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id));
                return (T)(dynamic)document;
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
                    return null;
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

        public async Task<T> GetItemAsync(Expression<Func<T, bool>> where, Expression<Func<T, T>> select)
        {
            try
            {
                IDocumentQuery<T> query = _client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId),
                    new FeedOptions { MaxItemCount = -1 })
                    .Where(where)
                    .Select(select)
                    .AsDocumentQuery();

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

        public async Task<bool> IsItemExistsByIdAsync(Guid id)
        {
            return await IsItemExistsByIdAsync(id.ToString());
        }

        public async Task<bool> IsItemExistsByIdAsync(string id)
        {
            try
            {
                Document document = await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id));
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

        public async Task<CosmosDBPageResponse<T>> GetItemsPagingAsync(int pageSize, string continuationToken)
        {
            IDocumentQuery<T> query = _client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId),
                new FeedOptions
                {
                    MaxItemCount = pageSize,
                    EnableCrossPartitionQuery = true,
                    RequestContinuation = continuationToken
                })
                .AsDocumentQuery();

            var response = new CosmosDBPageResponse<T>();

            while (query.HasMoreResults)
            {
                var result = await query.ExecuteNextAsync<T>();
                response.List = result.AsEnumerable<T>();
                response.ContinuationToken = result.ResponseContinuation;
                return response;
            }
            return null;
        }

        public async Task<CosmosDBPageResponse<T>> GetItemsPagingAsync(Expression<Func<T, bool>> where, int pageSize, string continuationToken)
        {
            IDocumentQuery<T> query = _client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId),
                    new FeedOptions
                    {
                        MaxItemCount = pageSize,
                        EnableCrossPartitionQuery = true,
                        RequestContinuation = continuationToken
                    })
                    .Where(where)
                    .AsDocumentQuery();

            var response = new CosmosDBPageResponse<T>();

            while (query.HasMoreResults)
            {
                var result = await query.ExecuteNextAsync<T>();
                response.List = result.AsEnumerable<T>();
                response.ContinuationToken = result.ResponseContinuation;
                return response;
            }
            return null;
        }

        public async Task<string> CreateItemAsync(T item)
        {
            try
            {
                await EnsureDatabaseAndCollectionExists();
                if(string.IsNullOrEmpty(item.Id) || ((Guid.TryParse(item.Id, out Guid guidResult) && guidResult == Guid.Empty)))
                    item.Id = Guid.NewGuid().ToString();
                item.CreationDate = DateTime.UtcNow;
                item.UpdateDate = DateTime.UtcNow;
                Document doc = await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId), item);
                return doc.Id;
            }
            catch (DocumentClientException e)
            {
                _logger.LogError("CreateItemAsync: DocumentClientException: " + e.Error.Message);
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError("CreateItemAsync: " + e.Message);
                return null;
            }
        }

        public async Task<string> CreateOrUpdateItemAsync(T item)
        {
            try
            {
                await EnsureDatabaseAndCollectionExists();
                if (string.IsNullOrEmpty(item.Id))
                    item.Id = Guid.NewGuid().ToString();
                item.CreationDate = DateTime.UtcNow;
                item.UpdateDate = DateTime.UtcNow;
                Document doc = await _client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId), item);
                return doc.Id;
            }
            catch (DocumentClientException e)
            {
                _logger.LogError("CreateItemAsync: DocumentClientException: " + e.Error.Message);
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError("CreateItemAsync: " + e.Message);
                return null;
            }
        }

        public async Task<bool> UpdateItemAsync(T item)
        {
            try
            {
                await EnsureDatabaseAndCollectionExists();

                if (item == null)
                    return false;

                item.UpdateDate = DateTime.UtcNow;

                var requestOptions = new RequestOptions()
                {
                    AccessCondition = new AccessCondition()
                    {
                        Type = AccessConditionType.IfMatch,
                        Condition = item.ETag
                    }
                };

                Document doc = await _client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, item.Id.ToString()), item, requestOptions);
                return true;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                    _logger.LogError($"UpdateItemAsync: Etag exception: {e.Error.Message}");
                else
                    _logger.LogError($"UpdateItemAsync: DocumentClientException: {e.Error.Message}");
                throw e;
            }
            catch (Exception e)
            {
                _logger.LogError($"UpdateItemAsync: {e.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteItemAsync(Guid id)
        {
            return await DeleteItemAsync(id.ToString());
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
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError("DeleteItemAsync: " + e.Message);
                return false;
            }
        }

        public async Task<bool> DeleteItemsAsync(Expression<Func<T, bool>> where)
        {
            try
            {
                await EnsureDatabaseAndCollectionExists();
                var results = await GetItemsAsync(where);
                foreach(var result in results)
                    await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, result.Id.ToString()));
                return true;
            }
            catch (DocumentClientException e)
            {
                _logger.LogError("DeleteItemAsync: DocumentClientException: " + e.Error.Message);
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError("DeleteItemAsync: " + e.Message);
                return false;
            }
        }

        public bool IsDocumentKeyNull(IEntityDAO entity)
        {
            return string.IsNullOrEmpty(entity.Id);
        }
    }
}
