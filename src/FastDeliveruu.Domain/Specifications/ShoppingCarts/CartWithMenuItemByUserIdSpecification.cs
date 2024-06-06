using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.ShoppingCarts;

public class CartWithMenuItemByUserIdSpecification : Specification<ShoppingCart>
{
    public CartWithMenuItemByUserIdSpecification(Guid localUserId)
        : base(sc => sc.LocalUserId == localUserId)
    {
        AddInclude(sc => sc.MenuItem);
    }
}
