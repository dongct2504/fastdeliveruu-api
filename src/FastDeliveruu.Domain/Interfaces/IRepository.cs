using FastDeliveruu.Domain.Specifications;

namespace FastDeliveruu.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> ListAllAsync(bool asNoTracking = false);

    Task<T?> GetAsync(int id);
    Task<T?> GetAsync(long id);
    Task<T?> GetAsync(Guid id);

    Task<int> GetCountAsync();

    Task AddAsync(T entity);
    Task DeleteAsync(T entity);
    Task DeleteRangeAsync(T entity);
    Task DeleteRangeAsync(IEnumerable<T> entities);

    Task<IEnumerable<T>> ListAllWithSpecAsync(ISpecification<T> spec, bool asNoTracking = false);

    Task<T?> GetWithSpecAsync(ISpecification<T> spec, bool asNoTracking = false);
}