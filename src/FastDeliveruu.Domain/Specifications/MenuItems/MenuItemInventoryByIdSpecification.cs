using FastDeliveruu.Domain.Entities.AutoGenEntities;

namespace FastDeliveruu.Domain.Specifications.MenuItems;

public class MenuItemInventoryByIdSpecification : Specification<MenuItemInventory>
{
    public MenuItemInventoryByIdSpecification(Guid menuItemId)
        : base(mi => mi.MenuItemId == menuItemId)
    {
    }
}
