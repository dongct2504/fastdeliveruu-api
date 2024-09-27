using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.Orders;

public class OrderByWardIdSpeicification : Specification<Order>
{
    public OrderByWardIdSpeicification(int wardId)
        : base(o => o.WardId == wardId)
    {
    }
}
