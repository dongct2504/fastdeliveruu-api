using FastDeliveruu.Domain.Extensions;

namespace FastDeliveruu.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> ListAllAsync(QueryOptions<T>? options = null);

    Task<int> GetCountAsync();

    Task<T?> GetAsync(int id);
    Task<T?> GetAsync(string id);
    Task<T?> GetAsync(QueryOptions<T> options);

    Task<T> AddAsync(T entity);

    Task DeleteAsync(T entity);
    Task DeleteRangeAsync(T entity);
    Task DeleteRangeAsync(List<T> entities);
}