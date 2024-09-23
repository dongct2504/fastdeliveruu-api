using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Infrastructure.Repositories;

public class CityRepository : Repository<City>, ICityRepository
{
    public CityRepository(FastDeliveruuDbContext context) : base(context)
    {
    }

    public void Update(City city)
    {
        _dbContext.Update(city);
    }
}
