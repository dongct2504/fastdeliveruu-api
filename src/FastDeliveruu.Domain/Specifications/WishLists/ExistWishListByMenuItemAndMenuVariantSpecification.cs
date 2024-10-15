using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.WishLists;

public class ExistWishListByMenuItemAndMenuVariantSpecification : Specification<WishList>
{
    public ExistWishListByMenuItemAndMenuVariantSpecification(Guid menuItemId, Guid? menuVariantId)
        : base(w => w.MenuItemId == menuItemId && 
            (menuVariantId == null || w.MenuVariantId == menuVariantId))
    {
    }
}
