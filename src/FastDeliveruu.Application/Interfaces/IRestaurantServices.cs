using FastDeliveruu.Domain.Entities;
using FluentResults;

namespace FastDeliveruu.Application.Interfaces;

public interface IRestaurantServices
{
    Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync();
    Task<Result<Restaurant>> GetRestaurantByIdAsync(Guid id);
    Task<Result<Restaurant>> GetRestaurantWithMenuItemsByIdAsync(Guid id);
    Task<Result<Restaurant>> GetRestaurantByNameAsync(string name);
    Task<Result<Restaurant>> GetRestaurantByPhoneNumberAsync(string phoneNumber);

    Task<Result<Guid>> CreateRestaurantAsync(Restaurant restaurant);
    Task<Result> UpdateRestaurantAsync(Guid id, Restaurant restaurant);
    Task<Result> DeleteRestaurantAsync(Guid id);
}
