using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities.Identity;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Infrastructure.Repositories;

public class ShipperRepository : Repository<Shipper>, IShipperRepository
{
    public ShipperRepository(FastDeliveruuDbContext context) : base(context)
    {
    }

    public void Update(Shipper shipper)
    {
        _dbContext.Update(shipper);
    }
}
