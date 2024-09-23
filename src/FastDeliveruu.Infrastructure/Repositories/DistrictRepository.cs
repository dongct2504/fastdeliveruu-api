using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Infrastructure.Repositories;

public class DistrictRepository : Repository<District>, IDistrictRepository
{
    public DistrictRepository(FastDeliveruuDbContext context) : base(context)
    {
    }

    public void Update(District district)
    {
        _dbContext.Update(district);
    }
}
