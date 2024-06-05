using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Data;

namespace FastDeliveruu.Infrastructure.Repositories;

public class RestaurantRepository : Repository<Restaurant>, IRestaurantRepository
{
    public RestaurantRepository(FastDeliveruuDbContext context) : base(context)
    {
    }

    public async Task UpdateAsync(Restaurant restaurant)
    {
        _dbContext.Update(restaurant);
        await _dbContext.SaveChangesAsync();
    }
}