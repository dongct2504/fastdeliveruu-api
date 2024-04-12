using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Infrastructure.Data;

namespace FastDeliveruu.Infrastructure.Repositories;

public class RestaurantRepository : Repository<Restaurant>, IRestaurantRepository
{
    public RestaurantRepository(FastdeliveruuContext context) : base(context)
    {
    }

    public async Task UpdateRestaurant(Restaurant restaurant)
    {
        _dbContext.Update(restaurant);
        await _dbContext.SaveChangesAsync();
    }
}