using FastDeliveruu.Domain.Extensions;

namespace FastDeliveruu.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> ListAllAsync(QueryOptions<T>? options = null, bool asNoTracking = false);

    Task<T?> GetAsync(int id);
    Task<T?> GetAsync(long id);
    Task<T?> GetAsync(Guid id);
    Task<T?> GetAsync(QueryOptions<T> options, bool asNoTracking = false);

    Task<int> GetCountAsync();

    Task AddAsync(T entity);
    Task DeleteAsync(T entity);
    Task DeleteRangeAsync(T entity);
    Task DeleteRangeAsync(List<T> entities);
}