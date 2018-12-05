using Edison.Common.Config;
using Edison.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Edison.Tests.Common.Helpers
{
    #pragma warning disable 1998
    public class InMemoryDBRepository<T> : ICosmosDBRepository<T> where T : class, IEntityDAO
    {
        private IList<T> _database;
        private ILogger<InMemoryDBRepository<T>> _logger;

        public InMemoryDBRepository(IList<T> dataSource, ILogger<InMemoryDBRepository<T>> logger)
        {
            _database = dataSource;
            _logger = logger;
        }

        public async Task<string> CreateItemAsync(T item)
        {
            try
            {
                if (string.IsNullOrEmpty(item.Id) || ((Guid.TryParse(item.Id, out Guid guidResult) && guidResult == Guid.Empty)))
                    item.Id = Guid.NewGuid().ToString();
                item.CreationDate = DateTime.UtcNow;
                item.UpdateDate = DateTime.UtcNow;

                if (_database.FirstOrDefault(p => p.Id == item.Id) != null)
                {
                    _logger.LogError($"An entry of the same Id already exist: '{item.Id}'");
                    return null;
                }

                _database.Add(item);

                return item.Id;
            }
            catch(Exception e)
            {
                _logger.LogError("CreateItemAsync: " + e.Message);
                return null;
            }
        }

        public async Task<string> CreateOrUpdateItemAsync(T item)
        {
            try
            {
                if (string.IsNullOrEmpty(item.Id) || ((Guid.TryParse(item.Id, out Guid guidResult) && guidResult == Guid.Empty)))
                    item.Id = Guid.NewGuid().ToString();
                item.CreationDate = DateTime.UtcNow;
                item.UpdateDate = DateTime.UtcNow;

                T itemToUpdate = _database.FirstOrDefault(p => p.Id == item.Id);
                if (itemToUpdate != null)
                    _database.Remove(itemToUpdate);

                _database.Add(item);

                return item.Id;
            }
            catch (Exception e)
            {
                _logger.LogError("CreateItemAsync: " + e.Message);
                return null;
            }
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            try
            {
                T itemToDelete = _database.FirstOrDefault(p => p.Id == id);
                if (itemToDelete != null)
                    _database.Remove(itemToDelete);
                else
                    return false;
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("DeleteItemAsync: " + e.Message);
                return false;
            }
        }

        public async Task<bool> DeleteItemAsync(Guid id)
        {
            return await DeleteItemAsync(id.ToString());
        }

        public async Task<bool> DeleteItemsAsync(Expression<Func<T, bool>> where)
        {
            try
            {
                var results = await GetItemsAsync(where);
                foreach (var result in results)
                    await DeleteItemAsync(result.Id);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("DeleteItemAsync: " + e.Message);
                return false;
            }
        }

        public Task EnsureDatabaseAndCollectionExists()
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetItemAsync(string id)
        {
            try
            {
                return _database.FirstOrDefault(p => p.Id == id);
            }
            catch (Exception e)
            {
                _logger.LogError("GetItemAsync: " + e.Message);
                return null;
            }
        }

        public async Task<T> GetItemAsync(Guid id)
        {
            return await GetItemAsync(id.ToString());
        }

        public async Task<T> GetItemAsync(Expression<Func<T, bool>> where)
        {
            try
            {
                return _database.FirstOrDefault(where.Compile());
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
                return _database.Where(where.Compile()).Select(select.Compile()).FirstOrDefault();
            }
            catch (Exception e)
            {
                _logger.LogError("GetItemAsync: " + e.Message);
                return null;
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> where, Expression<Func<T, T>> select)
        {
            try
            {
                return _database.Where(where.Compile()).Select(select.Compile());
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
                return _database.Where(where.Compile());
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
                return _database;
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
                return _database.Where(where.Compile()).OrderBy(orderBy.Compile()).Take(limit);
            }
            catch (Exception e)
            {
                _logger.LogError("GetItemsAsync: " + e.Message);
                return null;
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsyncOrderByDescending(Expression<Func<T, bool>> where, Expression<Func<T, DateTime>> orderByDescending, int limit = 100)
        {
            try
            {
                return _database.Where(where.Compile()).OrderByDescending(orderByDescending.Compile()).Take(limit);
            }
            catch (Exception e)
            {
                _logger.LogError("GetItemsAsync: " + e.Message);
                return null;
            }
        }

        public Task<CosmosDBPageResponse<T>> GetItemsPagingAsync(int pageSize, string continuationToken)
        {
            throw new NotImplementedException();
        }

        public Task<CosmosDBPageResponse<T>> GetItemsPagingAsync(Expression<Func<T, bool>> predicate, int pageSize, string continuationToken)
        {
            throw new NotImplementedException();
        }

        public bool IsDocumentKeyNull(IEntityDAO entity)
        {
            return string.IsNullOrEmpty(entity.Id);
        }

        public async Task<bool> IsItemExistsByIdAsync(string id)
        {
            try
            {
                T itemToCheck = _database.FirstOrDefault(p => p.Id == id);
                return itemToCheck != null;
            }
            catch (Exception e)
            {
                _logger.LogError("IsItemExistsByIdAsync: " + e.Message);
                throw e;
            }
        }

        public async Task<bool> IsItemExistsByIdAsync(Guid id)
        {
            return await IsItemExistsByIdAsync(id.ToString());
        }

        public async Task<bool> IsItemExistsByNonIdAsync(Expression<Func<T, bool>> where)
        {
            try
            {
                T itemToCheck = _database.FirstOrDefault(where.Compile());
                return itemToCheck != null;
            }
            catch (Exception e)
            {
                _logger.LogError("IsItemExistsByNonIdAsync: " + e.Message);
                throw e;
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

                T itemToUpdate = _database.FirstOrDefault(p => p.Id == item.Id);
                if (itemToUpdate == null)
                    return false;

                _database.Remove(itemToUpdate);
                _database.Add(item);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"UpdateItemAsync: {e.Message}");
                return false;
            }
        }
    }
}
