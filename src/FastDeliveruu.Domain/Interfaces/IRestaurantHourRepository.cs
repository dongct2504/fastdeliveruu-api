using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Interfaces;

public interface IRestaurantHourRepository : IRepository<RestaurantHour>
{
    Task UpdateAsync(RestaurantHour restaurantHour);
}
