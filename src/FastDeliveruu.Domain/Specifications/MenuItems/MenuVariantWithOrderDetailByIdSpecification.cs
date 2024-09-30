using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.MenuItems;

public class MenuVariantWithOrderDetailByIdSpecification : Specification<MenuVariant>
{
    public MenuVariantWithOrderDetailByIdSpecification(Guid id)
        : base(mr => mr.Id == id)
    {
        AddInclude(mr => mr.OrderDetails);
    }
}
