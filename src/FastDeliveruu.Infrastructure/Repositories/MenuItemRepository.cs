using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Data;

namespace FastDeliveruu.Infrastructure.Repositories;

public class MenuItemRepository : Repository<MenuItem>, IMenuItemRepository
{
    public MenuItemRepository(FastDeliveruuDbContext context) : base(context)
    {
    }

    public void Update(MenuItem menuItem)
    {
        _dbContext.Update(menuItem);
    }
}