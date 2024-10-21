using FastDeliveruu.Domain.Entities.AutoGenEntities;

namespace FastDeliveruu.Domain.Specifications.MenuItems;

public class MenuVariantInventoryByMenuVariantIdSpecification : Specification<MenuVariantInventory>
{
    public MenuVariantInventoryByMenuVariantIdSpecification(Guid menuVariantId)
        : base(mi => mi.MenuVariantId == menuVariantId)
    {
    }
}
