using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Application.Interfaces;

public interface IMenuItemServices
{
    Task<IEnumerable<MenuItem>> GetAllMenuItemsAsync(int page);
    Task<IEnumerable<MenuItem>> GetAllMenuItemsWithRestaurantGenreAsync(int page);
    Task<IEnumerable<MenuItem>> GetFilterMenuItemsWithRestaurantGenreAsync(
        int? genreId, int? restaurantId, int page);
    Task<IEnumerable<MenuItem>> SearchMenuItem(string name, int page);

    Task<MenuItem?> GetMenuItemByIdAsync(int id);
    Task<MenuItem?> GetMenuItemWithRestaurantGenreByIdAsync(int id);
    Task<MenuItem?> GetMenuItemByNameAsync(string name);
    Task<MenuItem?> GetMenuItemWithRestaurantGenreByNameAsync(string name);

    Task<int> GetTotalMenuItemsAsync();

    Task<int> CreateMenuItemAsync(MenuItem menuItem);
    Task UpdateMenuItemAsync(MenuItem menuItem);
    Task DeleteMenuItemAsync(int id);
}