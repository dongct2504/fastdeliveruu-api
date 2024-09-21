using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Infrastructure.Repositories;

public class RestaurantHourRepository : Repository<RestaurantHour>, IRestaurantHourRepository
{
    public RestaurantHourRepository(FastDeliveruuDbContext context) : base(context)
    {
    }

    public void Update(RestaurantHour restaurantHour)
    {
        _dbContext.Update(restaurantHour);
    }
}
