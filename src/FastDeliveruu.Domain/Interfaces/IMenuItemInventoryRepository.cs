using FastDeliveruu.Domain.Entities.AutoGenEntities;

namespace FastDeliveruu.Domain.Interfaces;

public interface IMenuItemInventoryRepository : IRepository<MenuItemInventory>
{
    void Update(MenuItemInventory menuItemInventory);
}
