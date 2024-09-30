using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.Orders;

public class OrderWithPaymentsById : Specification<Order>
{
    public OrderWithPaymentsById(Guid orderId)
        : base(o => o.Id == orderId)
    {
        AddInclude(o => o.Payments);
    }
}
