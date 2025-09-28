using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.Orders;

public class OrderWithPaymentsByPaymentOrderId : Specification<Order>
{
    public OrderWithPaymentsByPaymentOrderId(string paymentOrderId)
        : base(o => o.PaymentOrderId == paymentOrderId)
    {
        AddInclude(o => o.Payments);
        AddInclude(o => o.AppUser);
    }
}
