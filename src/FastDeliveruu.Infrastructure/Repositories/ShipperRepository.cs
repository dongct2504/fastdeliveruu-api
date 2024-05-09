using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Infrastructure.Data;

namespace FastDeliveruu.Infrastructure.Repositories;

public class ShipperRepository : Repository<Shipper>, IShipperRepository
{
    public ShipperRepository(Data.FastDeliveruuDbContext context) : base(context)
    {
    }

    public async Task UpdateAsync(Shipper shipper)
    {
        _dbContext.Update(shipper);
        await _dbContext.SaveChangesAsync();
    }
}