using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.WishLists;

public class WishListsByUserIdSpecification : Specification<WishList>
{
    public WishListsByUserIdSpecification(Guid userId)
        : base(w => w.AppUserId == userId)
    {
    }
}
