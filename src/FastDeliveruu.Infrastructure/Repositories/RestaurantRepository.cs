using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Infrastructure.Data;

namespace FastDeliveruu.Infrastructure.Repositories;

public class RestaurantRepository : Repository<Restaurant>, IRestaurantRepository
{
    public RestaurantRepository(FastDeliveruuContext context) : base(context)
    {
    }

    public async Task UpdateRestaurantAsync(Restaurant restaurant)
    {
        _dbContext.Update(restaurant);
        await _dbContext.SaveChangesAsync();
    }
}