using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Application.Interfaces;

public interface IRestaurantServices
{
    Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync(int page);
    Task<Restaurant?> GetRestaurantByIdAsync(int id);
    Task<RestaurantWithMenuItemsDto?> GetRestaurantWithMenuItemsByIdAsync(int id);
    Task<Restaurant?> GetRestaurantByNameAsync(string name);
    Task<Restaurant?> GetRestaurantByPhoneNumberAsync(string phoneNumber);

    Task<int> CreateRestaurantAsync(Restaurant restaurant);
    Task UpdateRestaurantAsync(Restaurant restaurant);
    Task DeleteRestaurantAsync(int id);
}
