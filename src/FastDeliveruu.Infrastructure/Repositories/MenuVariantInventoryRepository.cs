using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities.AutoGenEntities;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Infrastructure.Repositories;

public class MenuVariantInventoryRepository : Repository<MenuVariantInventory>, IMenuVariantInventoryRepository
{
    public MenuVariantInventoryRepository(FastDeliveruuDbContext context) : base(context)
    {
    }

    public void Update(MenuVariantInventory menuVariantInventory)
    {
        _dbContext.Update(menuVariantInventory);
    }
}
