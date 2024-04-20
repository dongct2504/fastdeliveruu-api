using FastDeliveruu.Domain.Entities;
using FluentResults;

namespace FastDeliveruu.Application.Interfaces;

public interface IMenuItemServices
{
    Task<IEnumerable<MenuItem>> GetAllMenuItemsAsync(int page);
    Task<IEnumerable<MenuItem>> GetAllFilterMenuItemsAsync(
        int? genreId, int? restaurantId, int page);
    Task<IEnumerable<MenuItem>> SearchMenuItem(string name, int page);

    Task<Result<MenuItem>> GetMenuItemByIdAsync(long id);
    Task<Result<MenuItem>> GetMenuItemWithRestaurantGenreByIdAsync(long id);
    Task<Result<MenuItem>> GetMenuItemByNameAsync(string name);
    Task<Result<MenuItem>> GetMenuItemWithRestaurantGenreByNameAsync(string name);

    Task<int> GetTotalMenuItemsAsync();

    Task<Result<long>> CreateMenuItemAsync(MenuItem menuItem);
    Task<Result> UpdateMenuItemAsync(long id, MenuItem menuItem);
    Task<Result> DeleteMenuItemAsync(long id);
}