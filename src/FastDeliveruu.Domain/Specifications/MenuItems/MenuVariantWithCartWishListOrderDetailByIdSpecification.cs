using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.MenuItems;

public class MenuVariantWithCartWishListOrderDetailByIdSpecification : Specification<MenuVariant>
{
    public MenuVariantWithCartWishListOrderDetailByIdSpecification(Guid id)
        : base(mv => mv.Id == id)
    {
        AddInclude(mv => mv.ShoppingCarts);
        AddInclude(mv => mv.WishLists);
        AddInclude(mv => mv.OrderDetails);
    }
}
