using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Interfaces;

public interface IRestaurantRepository : IRepository<Restaurant>
{
    Task UpdateAsync(Restaurant restaurant);
}