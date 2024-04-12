using System.Linq.Expressions;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly FastdeliveruuContext _dbContext;

    private readonly DbSet<T> _dbSet;

    private int count;

    public Repository(FastdeliveruuContext context)
    {
        _dbContext = context;
        _dbSet = _dbContext.Set<T>();
    }

    public async Task<IEnumerable<T>> ListAllAsync(QueryOptions<T>? options = null)
    {
        if (options != null)
        {
            return await BuildQuery(options).ToListAsync();
        }

        return await _dbSet.ToListAsync();
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

    public virtual async Task<T?> GetAsync(int id) => await _dbSet.FindAsync(id);
    public virtual async Task<T?> GetAsync(string id) => await _dbSet.FindAsync(id);
    public virtual async Task<T?> GetAsync(QueryOptions<T> options) =>
        await BuildQuery(options).FirstOrDefaultAsync();

    private IQueryable<T> BuildQuery(QueryOptions<T> options)
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

        if (options.HasOrderBy)
        {
            query = query.OrderBy(options.OrderBy);
        }

        if (options.HasPaging)
        {
            query = query.Skip(PagingConstants.DefaultPageSize * (options.PageNumber - 1))
                .Take(PagingConstants.DefaultPageSize);
        }

        return query;
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        return entity;
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
}