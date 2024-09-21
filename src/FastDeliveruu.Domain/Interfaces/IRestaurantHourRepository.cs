using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Interfaces;

public interface IRestaurantHourRepository : IRepository<RestaurantHour>
{
    void Update(RestaurantHour restaurantHour);
}
