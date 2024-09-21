using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    void Update(Order order);
}