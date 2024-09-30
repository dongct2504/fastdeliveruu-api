using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Interfaces;

public interface IOrderDetailRepository : IRepository<OrderDetail>
{
    void Update(OrderDetail orderDetail);
}
