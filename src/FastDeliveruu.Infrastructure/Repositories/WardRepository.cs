using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Infrastructure.Repositories;

public class WardRepository : Repository<Ward>, IWardRepository
{
    public WardRepository(FastDeliveruuDbContext context) : base(context)
    {
    }

    public void Update(Ward ward)
    {
        _dbContext.Update(ward);
    }
}
