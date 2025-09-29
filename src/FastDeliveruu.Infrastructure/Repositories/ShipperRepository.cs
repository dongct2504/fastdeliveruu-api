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

    public async Task<Shipper?> FindNearestShipper(decimal latitude, decimal longitude)
    {
        double lat = (double)latitude;
        double lon = (double)longitude;

        // dùng Haversine
        return await _dbContext.Shippers
            .OrderBy(s =>
                6371 * 2 * Math.Asin(Math.Sqrt(
                    Math.Pow(Math.Sin((Math.PI / 180) * ((double)s.Latitude - lat) / 2), 2) +
                    Math.Cos(Math.PI / 180 * lat) *
                    Math.Cos(Math.PI / 180 * (double)s.Latitude) *
                    Math.Pow(Math.Sin(Math.PI / 180 * ((double)s.Longitude - lon) / 2), 2)
                ))
            )
            .FirstOrDefaultAsync();
    }

    public void Update(Shipper shipper)
    {
        _dbContext.Update(shipper);
    }
}
