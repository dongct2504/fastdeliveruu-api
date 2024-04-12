using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;

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

    public async Task<Restaurant?> GetRestaurantByIdAsync(int id)
    {
        return await _restaurantRepository.GetAsync(id);
    }

    public async Task<Restaurant?> GetRestaurantWithMenuItemsByIdAsync(int id)
    {
        QueryOptions<Restaurant> options = new QueryOptions<Restaurant>
        {
            SetIncludes = "MenuItems",
            Where = r => r.RestaurantId == id
        };

        return await _restaurantRepository.GetAsync(options);
    }

    public async Task<Restaurant?> GetRestaurantByNameAsync(string name)
    {
        QueryOptions<Restaurant> options = new QueryOptions<Restaurant>
        {
            SetIncludes = "MenuItems",
            Where = r => r.Name == name
        };

        return await _restaurantRepository.GetAsync(options);
    }

    public async Task<Restaurant?> GetRestaurantByPhoneNumberAsync(string phoneNumber)
    {
        QueryOptions<Restaurant> options = new QueryOptions<Restaurant>
        {
            SetIncludes = "MenuItems",
            Where = r => r.PhoneNumber == phoneNumber
        };

        return await _restaurantRepository.GetAsync(options);
    }

    public async Task<int> CreateRestaurantAsync(Restaurant restaurant)
    {
        Restaurant createdRestaurant = await _restaurantRepository.AddAsync(restaurant);

        return createdRestaurant.RestaurantId;
    }

    public async Task UpdateRestaurantAsync(Restaurant restaurant)
    {
        await _restaurantRepository.UpdateRestaurant(restaurant);
    }

    public async Task DeleteRestaurantAsync(int id)
    {
        Restaurant? restaurant = await _restaurantRepository.GetAsync(id);
        if (restaurant != null)
        {
            await _restaurantRepository.DeleteAsync(restaurant);
        }
    }
}
