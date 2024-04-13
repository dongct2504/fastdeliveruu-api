using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Infrastructure.Data;

namespace FastDeliveruu.Infrastructure.Repositories;

public class MenuItemRepository : Repository<MenuItem>, IMenuItemRepository
{
    public MenuItemRepository(FastdeliveruuContext context) : base(context)
    {
    }

    public async Task UpdateMenuItem(MenuItem menuItem)
    {
        _dbContext.Update(menuItem);
        await _dbContext.SaveChangesAsync();
    }
}