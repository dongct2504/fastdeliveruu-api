using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Interfaces;

public interface IRestaurantRepository : IRepository<Restaurant>
{
    void Update(Restaurant restaurant);
}