using FastDeliveruu.Domain.Entities;
using FluentResults;

namespace FastDeliveruu.Application.Interfaces;

public interface IMenuItemServices
{
    Task<IEnumerable<MenuItem>> GetAllMenuItemsAsync(int page);
    Task<IEnumerable<MenuItem>> GetFilterMenuItemsWithRestaurantGenreAsync(
        int? genreId, Guid? restaurantId, int page);
    Task<IEnumerable<MenuItem>> SearchMenuItem(string name, int page);

    Task<Result<MenuItem>> GetMenuItemByIdAsync(Guid id);
    Task<Result<MenuItem>> GetMenuItemWithRestaurantGenreByIdAsync(Guid id);
    Task<Result<MenuItem>> GetMenuItemByNameAsync(string name);
    Task<Result<MenuItem>> GetMenuItemWithRestaurantGenreByNameAsync(string name);

    Task<int> GetTotalMenuItemsAsync();

    Task<Result<Guid>> CreateMenuItemAsync(MenuItem menuItem);
    Task<Result> UpdateMenuItemAsync(Guid id, MenuItem menuItem);
    Task<Result> DeleteMenuItemAsync(Guid id);
}