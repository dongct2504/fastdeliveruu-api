using FastDeliveruu.Domain.Entities.AutoGenEntities;

namespace FastDeliveruu.Domain.Specifications.MenuItems;

public class MenuItemInventoryByMenuItemIdSpecification : Specification<MenuItemInventory>
{
    public MenuItemInventoryByMenuItemIdSpecification(Guid menuItemId)
        : base(mi => mi.MenuItemId == menuItemId)
    {
    }
}
