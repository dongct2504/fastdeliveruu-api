using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.Orders;

public class OrderByCityIdSpecification : Specification<Order>
{
    public OrderByCityIdSpecification(int cityId)
        : base(o => o.CityId == cityId)
    {
    }
}
