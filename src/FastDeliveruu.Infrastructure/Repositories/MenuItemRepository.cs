using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Data;

namespace FastDeliveruu.Infrastructure.Repositories;

public class MenuItemRepository : Repository<MenuItem>, IMenuItemRepository
{
    public MenuItemRepository(FastDeliveruuDbContext context) : base(context)
    {
    }

    public async Task UpdateAsync(MenuItem menuItem)
    {
        _dbContext.Update(menuItem);
        await _dbContext.SaveChangesAsync();
    }
}