using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task UpdateAsync(Order order);
}