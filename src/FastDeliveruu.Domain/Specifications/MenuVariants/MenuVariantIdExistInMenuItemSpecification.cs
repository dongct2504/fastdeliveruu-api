using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.MenuVariants;

public class MenuVariantIdExistInMenuItemSpecification : Specification<MenuVariant>
{
    public MenuVariantIdExistInMenuItemSpecification(Guid menuItemId, Guid? menuVariantId)
        : base(mv => mv.Id == menuVariantId && mv.MenuItemId == menuItemId)
    {
    }
}
