using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Infrastructure.Repositories;

public class RestaurantHourRepository : Repository<RestaurantHour>, IRestaurantHourRepository
{
    public RestaurantHourRepository(FastDeliveruuDbContext context) : base(context)
    {
    }

    public async Task UpdateAsync(RestaurantHour restaurantHour)
    {
        _dbContext.Update(restaurantHour);
        await _dbContext.SaveChangesAsync();
    }
}
