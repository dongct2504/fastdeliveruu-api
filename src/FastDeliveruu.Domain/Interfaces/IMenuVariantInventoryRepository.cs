using FastDeliveruu.Domain.Entities.AutoGenEntities;

namespace FastDeliveruu.Domain.Interfaces;

public interface IMenuVariantInventoryRepository : IRepository<MenuVariantInventory>
{
    void Update(MenuVariantInventory menuVariantInventory);
}
