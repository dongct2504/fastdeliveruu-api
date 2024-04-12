using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Application.Dtos;

public class MenuItemServices : IMenuItemServices
{
    private readonly IMenuItemRepository _menuItemRepository;

    public MenuItemServices(IMenuItemRepository menuItemRepository)
    {
        _menuItemRepository = menuItemRepository;
    }

    public async Task<IEnumerable<MenuItem>> GetAllMenuItemsAsync(int page)
    {
        QueryOptions<MenuItem> options = new QueryOptions<MenuItem>
        {
            PageNumber = page
        };

        return await _menuItemRepository.ListAllAsync(options);
    }

    public async Task<IEnumerable<MenuItem>> GetAllMenuItemsWithRestaurantGenreAsync(
        int page)
    {
        QueryOptions<MenuItem> options = new QueryOptions<MenuItem>
        {
            SetIncludes = "Restaurant, Genre",
            PageNumber = page
        };

        return await _menuItemRepository.ListAllAsync(options);
    }

    public async Task<IEnumerable<MenuItem>> GetFilterMenuItemsWithRestaurantGenreAsync(
        int? genreId, int? restaurantId, int page)
    {
        QueryOptions<MenuItem> options = new QueryOptions<MenuItem>
        {
            SetIncludes = "Restaurant, Genre",
            PageNumber = page
        };

        if (genreId != null)
        {
            options.Where = mi => mi.GenreId == genreId;
        }

        if (restaurantId != null)
        {
            options.Where = mi => mi.RestaurantId == restaurantId;
        }

        return await _menuItemRepository.ListAllAsync(options);
    }

    public async Task<IEnumerable<MenuItem>> SearchMenuItem(string name, int page)
    {
        QueryOptions<MenuItem> options = new QueryOptions<MenuItem>
        {
            SetIncludes = "Restaurant, Genre",
            Where = mi => mi.Name.Contains(name),
            PageNumber = page
        };

        return await _menuItemRepository.ListAllAsync(options);
    }

    public async Task<MenuItem?> GetMenuItemByIdAsync(int id)
    {
        return await _menuItemRepository.GetAsync(id);
    }

    public async Task<MenuItem?> GetMenuItemWithRestaurantGenreByIdAsync(int id)
    {
        QueryOptions<MenuItem> options = new QueryOptions<MenuItem>
        {
            SetIncludes = "Restaurant, Genre",
            Where = g => g.MenuItemId == id
        };

        return await _menuItemRepository.GetAsync(options);
    }

    public async Task<MenuItem?> GetMenuItemByNameAsync(string name)
    {
        QueryOptions<MenuItem> options = new QueryOptions<MenuItem>
        {
            Where = g => g.Name == name
        };

        return await _menuItemRepository.GetAsync(options);
    }

    public async Task<MenuItem?> GetMenuItemWithRestaurantGenreByNameAsync(string name)
    {
        QueryOptions<MenuItem> options = new QueryOptions<MenuItem>
        {
            SetIncludes = "Restaurant, Genre",
            Where = g => g.Name == name
        };

        return await _menuItemRepository.GetAsync(options);
    }

    public async Task<int> GetTotalMenuItemsAsync()
    {
        return await _menuItemRepository.GetCountAsync();
    }

    public async Task<int> CreateMenuItemAsync(MenuItem menuItem)
    {
        MenuItem createdMenuItem = await _menuItemRepository.AddAsync(menuItem);

        return createdMenuItem.MenuItemId;
    }

    public async Task UpdateMenuItemAsync(MenuItem menuItem)
    {
        await _menuItemRepository.UpdateMenuItem(menuItem);
    }

    public async Task DeleteMenuItemAsync(int id)
    {
        MenuItem? menuItem = await _menuItemRepository.GetAsync(id);
        if (menuItem != null)
        {
            await _menuItemRepository.DeleteAsync(menuItem);
        }
    }
}