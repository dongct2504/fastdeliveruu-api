using FastDeliveruu.Domain.Specifications;

namespace FastDeliveruu.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> ListAllAsync(bool asNoTracking = false);

    Task<T?> GetAsync(int id);
    Task<T?> GetAsync(long id);
    Task<T?> GetAsync(Guid id);

    Task<int> GetCountAsync();

    void Add(T entity);
    void Delete(T entity);
    void DeleteRange(T entity);
    void DeleteRange(IEnumerable<T> entities);

    Task<IEnumerable<T>> ListAllWithSpecAsync(ISpecification<T> spec, bool asNoTracking = false);

    Task<T?> GetWithSpecAsync(ISpecification<T> spec, bool asNoTracking = false);
}