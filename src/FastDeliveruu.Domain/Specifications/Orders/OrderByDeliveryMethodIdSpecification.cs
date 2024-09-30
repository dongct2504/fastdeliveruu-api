using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.Orders;

public class OrderByDeliveryMethodIdSpecification : Specification<Order>
{
    public OrderByDeliveryMethodIdSpecification(int deliveryMethodId)
        : base(o => o.DeliveryMethodId == deliveryMethodId)
    {
    }
}
