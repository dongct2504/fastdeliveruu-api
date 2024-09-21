using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Infrastructure.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(FastDeliveruuDbContext context) : base(context)
    {
    }

    public void Update(Order order)
    {
        _dbContext.Update(order);
    }
}