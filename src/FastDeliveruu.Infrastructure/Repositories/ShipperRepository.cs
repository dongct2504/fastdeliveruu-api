using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Data;

namespace FastDeliveruu.Infrastructure.Repositories;

public class ShipperRepository : Repository<Shipper>, IShipperRepository
{
    public ShipperRepository(FastDeliveruuDbContext context) : base(context)
    {
    }

    public async Task UpdateAsync(Shipper shipper)
    {
        _dbContext.Update(shipper);
        await _dbContext.SaveChangesAsync();
    }
}