using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.MenuItems;

public class MenuVariantNameExistInMenuItemSpecification : Specification<MenuVariant>
{
    public MenuVariantNameExistInMenuItemSpecification(Guid menuItemId, string name)
        : base(mv => mv.MenuItemId == menuItemId && mv.VarietyName == name)
    {
    }
}
