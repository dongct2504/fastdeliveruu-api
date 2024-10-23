using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities.Identity;
using FastDeliveruu.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Infrastructure.Repositories;

public class ShipperRepository : Repository<Shipper>, IShipperRepository
{
    public ShipperRepository(FastDeliveruuDbContext context) : base(context)
    {
    }

    public Task<Shipper?> FindNearestShipper(decimal latitude, decimal longitude)
    {
        return _dbContext.Shippers.FirstOrDefaultAsync();
    }

    public void Update(Shipper shipper)
    {
        _dbContext.Update(shipper);
    }
}
