using FastDeliveruu.Domain.Entities.AutoGenEntities;

namespace FastDeliveruu.Domain.Specifications.MenuItems;

public class MenuVariantInventoryByIdSpecification : Specification<MenuVariantInventory>
{
    public MenuVariantInventoryByIdSpecification(Guid menuVariantId)
        : base(mi => mi.MenuVariantId == menuVariantId)
    {
    }
}
