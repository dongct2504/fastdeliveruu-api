using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.ShoppingCarts;

public class CartByUserIdAndMenuItemIdSpecification : Specification<ShoppingCart>
{
    public CartByUserIdAndMenuItemIdSpecification(Guid userId, Guid menuItemId)
        : base (sc => sc.LocalUserId == userId && sc.MenuItemId == menuItemId)
    {
    }
}
