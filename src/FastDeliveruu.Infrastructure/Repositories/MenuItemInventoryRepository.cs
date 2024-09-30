using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities.AutoGenEntities;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Infrastructure.Repositories;

public class MenuItemInventoryRepository : Repository<MenuItemInventory>, IMenuItemInventoryRepository
{
    public MenuItemInventoryRepository(FastDeliveruuDbContext context) : base(context)
    {
    }

    public void Update(MenuItemInventory menuItemInventory)
    {
        _dbContext.Update(menuItemInventory);
    }
}
