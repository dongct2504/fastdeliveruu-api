using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Specifications;

namespace FastDeliveruu.Infrastructure.Specifications.MenuVariants;

public class MenuVariantExistInMenuItemSpecification : Specification<MenuVariant>
{
    public MenuVariantExistInMenuItemSpecification(Guid menuItemId, string name)
        : base(mi => mi.VarietyName == name && mi.MenuItemId == menuItemId)
    {
    }
}
