using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.ShoppingCarts;

public class CartByUserIdSpecification : Specification<ShoppingCart>
{
    public CartByUserIdSpecification(Guid userId)
        : base(sc => sc.LocalUserId == userId)
    {
    }
}
