using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Infrastructure.Repositories;

public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
{
    public OrderDetailRepository(FastDeliveruuDbContext context) : base(context)
    {
    }

    public void Update(OrderDetail orderDetail)
    {
        _dbContext.Update(orderDetail);
    }
}
