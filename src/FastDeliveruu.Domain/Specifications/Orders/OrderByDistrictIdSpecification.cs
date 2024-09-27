using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.Orders;

public class OrderByDistrictIdSpecification : Specification<Order>
{
    public OrderByDistrictIdSpecification(int districtId)
        : base (o => o.DistrictId == districtId)
    {
    }
}
