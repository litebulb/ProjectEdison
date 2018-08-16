using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Edison.Common.Managers
{
    public interface ICosmosDBRepository<T>
    {
        Task<T> GetItemAsync(string id);
        Task<T> GetItemAsync(Expression<Func<T, bool>> predicate);
        Task<bool> IsItemExistsByIdAsync(string Id);
        Task<bool> IsItemExistsByNonIdAsync(Expression<Func<T, bool>> where);
        Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> select);
        Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetItemsAsyncOrderByAscending(Expression<Func<T, bool>> where, Expression<Func<T, DateTime>> orderBy, int limit = 100);
        Task<IEnumerable<T>> GetItemsAsyncOrderByDescending(Expression<Func<T, bool>> where, Expression<Func<T, DateTime>> orderByDescending, int limit = 100);
        Task<IEnumerable<T>> GetItemsAsync();
        Task<string> CreateItemAsync(T item);
        Task<bool> UpdateItemAsync(string id, T item);
        Task<bool> DeleteItemAsync(string id);
        Task<bool> DeleteItemsAsync(Expression<Func<T, bool>> where);
    }
}
