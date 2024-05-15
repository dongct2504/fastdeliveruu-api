using System.Linq.Expressions;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IEnumerable<T>> ListAllAsync(QueryOptions<T>? options = null, bool asNoTracking = false)
    {
        if (options != null)
        {
            return await BuildQuery(options, asNoTracking).ToListAsync();
        }

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

    public virtual async Task<T?> GetAsync(QueryOptions<T> options, bool asNoTracking = false) =>
        await BuildQuery(options, asNoTracking).FirstOrDefaultAsync();

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

    public async Task DeleteRangeAsync(List<T> entities)
    {
        _dbSet.RemoveRange(entities);
        await _dbContext.SaveChangesAsync();
    }

    private IQueryable<T> BuildQuery(QueryOptions<T> options, bool asNoTracking)
    {
        IQueryable<T> query = _dbSet; // ex: _context.Books;

        if (!options.Tracked)
        {
            query = query.AsNoTracking();
        }

        if (options.HasInclude)
        {
            foreach (string include in options.GetIncludes)
            {
                query = query.Include(include);
            }
        }

        if (options.HasWhereClause)
        {
            foreach (Expression<Func<T, bool>> expression in options.WhereClauses)
            {
                query = query.Where(expression);
            }
            count = query.Count(); // get filter count
        }

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        if (options.HasPaging)
        {
            query = query.Skip(options.PageSize * (options.PageNumber - 1))
                .Take(options.PageSize);
        }

        if (options.HasOrderBy)
        {
            query = query.OrderBy(options.OrderBy);
        }

        return query;
    }
}