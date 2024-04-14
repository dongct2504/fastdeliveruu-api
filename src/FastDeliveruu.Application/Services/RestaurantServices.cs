using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;

namespace FastDeliveruu.Application.Services;

public class RestaurantServices : IRestaurantServices
{
    private readonly IRestaurantRepository _restaurantRepository;

    public RestaurantServices(IRestaurantRepository restaurantRepository)
    {
        _restaurantRepository = restaurantRepository;
    }

    public async Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync()
    {
        return await _restaurantRepository.ListAllAsync();
    }

    public async Task<Result<Restaurant>> GetRestaurantByIdAsync(Guid id)
    {
        Restaurant? restaurant = await _restaurantRepository.GetAsync(id);
        if (restaurant == null)
        {
            return Result.Fail<Restaurant>(new NotFoundError($"the requested restaurant '{id}' is not found."));
        }

        return restaurant;
    }

    public async Task<Result<Restaurant>> GetRestaurantWithMenuItemsByIdAsync(Guid id)
    {
        QueryOptions<Restaurant> options = new QueryOptions<Restaurant>
        {
            SetIncludes = "MenuItems",
            Where = r => r.RestaurantId == id
        };

        Restaurant? restaurant = await _restaurantRepository.GetAsync(options);
        if (restaurant == null)
        {
            return Result.Fail<Restaurant>(new NotFoundError($"the requested restaurant '{id}' is not found."));
        }

        return restaurant;
    }

    public async Task<Result<Restaurant>> GetRestaurantByNameAsync(string name)
    {
        QueryOptions<Restaurant> options = new QueryOptions<Restaurant>
        {
            SetIncludes = "MenuItems",
            Where = r => r.Name == name
        };

        Restaurant? restaurant = await _restaurantRepository.GetAsync(options);
        if (restaurant == null)
        {
            return Result.Fail<Restaurant>(
                new NotFoundError($"the requested restaurant '{name}' is not found."));
        }

        return restaurant;
    }

    public async Task<Result<Restaurant>> GetRestaurantByPhoneNumberAsync(string phoneNumber)
    {
        QueryOptions<Restaurant> options = new QueryOptions<Restaurant>
        {
            SetIncludes = "MenuItems",
            Where = r => r.PhoneNumber == phoneNumber
        };

        Restaurant? restaurant = await _restaurantRepository.GetAsync(options);
        if (restaurant == null)
        {
            return Result.Fail<Restaurant>(
                new NotFoundError($"the requested restaurant '{phoneNumber}' is not found."));
        }

        return restaurant;
    }

    public async Task<Result<Guid>> CreateRestaurantAsync(Restaurant restaurant)
    {
        QueryOptions<Restaurant> options = new QueryOptions<Restaurant>
        {
            Where = r => r.Name == restaurant.Name && r.PhoneNumber == restaurant.PhoneNumber
        };

        Restaurant? isRestaurantExist = await _restaurantRepository.GetAsync(options);
        if (isRestaurantExist != null)
        {
            return Result.Fail<Guid>(
                new DuplicateError($"the requested restaurant '{restaurant.Name}' is already exists."));
        }

        Restaurant createdRestaurant = await _restaurantRepository.AddAsync(restaurant);

        return createdRestaurant.RestaurantId;
    }

    public async Task<Result> UpdateRestaurantAsync(Guid id, Restaurant restaurant)
    {
        Restaurant? isRestaurantExist = await _restaurantRepository.GetAsync(id);
        if (isRestaurantExist == null)
        {
            return Result.Fail(
                new NotFoundError($"the requested restaurant '{id}' is not found."));
        }

        await _restaurantRepository.UpdateRestaurantAsync(restaurant);

        return Result.Ok();
    }

    public async Task<Result> DeleteRestaurantAsync(Guid id)
    {
        Restaurant? isRestaurantExist = await _restaurantRepository.GetAsync(id);
        if (isRestaurantExist == null)
        {
            return Result.Fail(
                new NotFoundError($"the requested restaurant '{id}' is not found."));
        }

        await _restaurantRepository.DeleteAsync(isRestaurantExist);

        return Result.Ok();
    }
}
