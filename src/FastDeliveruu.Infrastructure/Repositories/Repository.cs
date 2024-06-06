using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Data;
using Microsoft.EntityFrameworkCore;
using FastDeliveruu.Domain.Specifications;

namespace FastDeliveruu.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly FastDeliveruuDbContext _dbContext;

    private readonly DbSet<T> _dbSet;

    private int count;

    public Repository(FastDeliveruuDbContext context)
    {
        _dbContext = context;
        _dbSet = _dbContext.Set<T>();
    }

    public async Task<IEnumerable<T>> ListAllAsync(bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        return await _dbSet.ToListAsync();
    }

    public async Task<T?> GetAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T?> GetAsync(long id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T?> GetAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    // if count = 0 (Where is not use) then use _dbSet.CountAsync()
    public async Task<int> GetCountAsync()
    {
        if (count > 0)
        {
            return count;
        }

        return await _dbSet.CountAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteRangeAsync(T entity)
    {
        _dbSet.RemoveRange(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<T>> ListAllWithSpecAsync(ISpecification<T> spec, bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await ApplySpecification(spec).AsNoTracking().ToListAsync();
        }

        return await ApplySpecification(spec).ToListAsync();
    }

    public async Task<T?> GetWithSpecAsync(ISpecification<T> spec, bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await ApplySpecification(spec).AsNoTracking().FirstOrDefaultAsync();
        }

        return await ApplySpecification(spec).FirstOrDefaultAsync();
    }

    private IQueryable<T> ApplySpecification(ISpecification<T> spec)
    {
        return SpecificationEvaluator<T>.GetQuery(_dbSet.AsQueryable(), spec);
    }
}