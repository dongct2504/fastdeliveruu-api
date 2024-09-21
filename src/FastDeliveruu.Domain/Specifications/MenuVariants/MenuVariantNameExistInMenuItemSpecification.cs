using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Specifications;

namespace FastDeliveruu.Infrastructure.Specifications.MenuVariants;

public class MenuVariantNameExistInMenuItemSpecification : Specification<MenuVariant>
{
    public MenuVariantNameExistInMenuItemSpecification(Guid menuItemId, string name)
        : base(mv => mv.VarietyName == name && mv.MenuItemId == menuItemId)
    {
    }
}
