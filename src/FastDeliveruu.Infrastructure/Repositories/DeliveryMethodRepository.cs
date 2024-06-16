using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Infrastructure.Repositories;

public class DeliveryMethodRepository : Repository<DeliveryMethod>, IDeliveryMethodRepository
{
    public DeliveryMethodRepository(FastDeliveruuDbContext context) : base(context)
    {
    }

    public async Task UpdateAsync(DeliveryMethod deliveryMethod)
    {
        _dbContext.Update(deliveryMethod);
        await _dbContext.SaveChangesAsync();
    }
}
