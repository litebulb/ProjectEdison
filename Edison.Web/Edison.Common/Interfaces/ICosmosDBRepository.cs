using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Edison.Common.Interfaces
{
    public interface ICosmosDBRepository<T>
    {
        Task<T> GetItemAsync(Guid id);
        Task<T> GetItemAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetItemAsync(Expression<Func<T, bool>> where, Expression<Func<T, T>> select);
        Task<bool> IsItemExistsByIdAsync(Guid id);
        Task<bool> IsItemExistsByNonIdAsync(Expression<Func<T, bool>> where);
        Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> select);
        Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetItemsAsyncOrderByAscending(Expression<Func<T, bool>> where, Expression<Func<T, DateTime>> orderBy, int limit = 100);
        Task<IEnumerable<T>> GetItemsAsyncOrderByDescending(Expression<Func<T, bool>> where, Expression<Func<T, DateTime>> orderByDescending, int limit = 100);
        Task<CosmosDBPageResponse<T>> GetItemsPagingAsync(int pageSize, string continuationToken);
        Task<CosmosDBPageResponse<T>> GetItemsPagingAsync(Expression<Func<T, bool>> predicate, int pageSize, string continuationToken);
        Task<IEnumerable<T>> GetItemsAsync();
        Task<Guid> CreateItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(Guid id);
        Task<bool> DeleteItemsAsync(Expression<Func<T, bool>> where);
    }
}
