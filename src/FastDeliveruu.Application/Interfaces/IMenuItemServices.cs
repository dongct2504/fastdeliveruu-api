using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Application.Interfaces;

public interface IMenuItemServices
{
    Task<IEnumerable<MenuItem>> GetAllMenuItemsAsync(int page);
    Task<IEnumerable<MenuItemWithRestaurantGenreDto>> GetAllMenuItemsWithRestaurantGenreAsync(int page);
    Task<IEnumerable<MenuItemWithRestaurantGenreDto>> GetFilterMenuItemsWithRestaurantGenreAsync(
        int? genreId, int? restaurantId, int page);

    Task<MenuItem?> GetMenuItemByIdAsync(int id);
    Task<MenuItemWithRestaurantGenreDto?> GetMenuItemWithRestaurantGenreByIdAsync(int id);
    Task<MenuItem?> GetMenuItemByNameAsync(string name);
    Task<MenuItemWithRestaurantGenreDto?> GetMenuItemWithRestaurantGenreByNameAsync(string name);

    Task<int> CreateMenuItemAsync(MenuItem menuItem);
    Task UpdateMenuItemAsync(MenuItem menuItem);
    Task DeleteMenuItemAsync(int id);
}