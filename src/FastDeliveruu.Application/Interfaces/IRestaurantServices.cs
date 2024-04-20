using FastDeliveruu.Domain.Entities;
using FluentResults;

namespace FastDeliveruu.Application.Interfaces;

public interface IRestaurantServices
{
    Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync();
    Task<Result<Restaurant>> GetRestaurantByIdAsync(int id);
    Task<Result<Restaurant>> GetRestaurantWithMenuItemsByIdAsync(int id);
    Task<Result<Restaurant>> GetRestaurantByNameAsync(string name);
    Task<Result<Restaurant>> GetRestaurantByPhoneNumberAsync(string phoneNumber);

    Task<Result<int>> CreateRestaurantAsync(Restaurant restaurant);
    Task<Result> UpdateRestaurantAsync(int id, Restaurant restaurant);
    Task<Result> DeleteRestaurantAsync(int id);
}
