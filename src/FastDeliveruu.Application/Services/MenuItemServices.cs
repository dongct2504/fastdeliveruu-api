using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;

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
            PageNumber = page,
            PageSize = PagingConstants.DefaultPageSize
        };

        return await _menuItemRepository.ListAllAsync(options);
    }

    public async Task<IEnumerable<MenuItem>> GetFilterMenuItemsWithRestaurantGenreAsync(
        int? genreId, Guid? restaurantId, int page)
    {
        QueryOptions<MenuItem> options = new QueryOptions<MenuItem>
        {
            SetIncludes = "Restaurant, Genre",
            PageNumber = page,
            PageSize = PagingConstants.DefaultPageSize
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

    public async Task<Result<MenuItem>> GetMenuItemByIdAsync(Guid id)
    {
        MenuItem? menuItem = await _menuItemRepository.GetAsync(id);
        if (menuItem == null)
        {
            return Result.Fail<MenuItem>(new NotFoundError($"the requested menu item '{id}' is not found."));
        }

        return menuItem;
    }

    public async Task<Result<MenuItem>> GetMenuItemWithRestaurantGenreByIdAsync(Guid id)
    {
        QueryOptions<MenuItem> options = new QueryOptions<MenuItem>
        {
            SetIncludes = "Restaurant, Genre",
            Where = g => g.MenuItemId == id
        };

        MenuItem? menuItem = await _menuItemRepository.GetAsync(options);
        if (menuItem == null)
        {
            return Result.Fail<MenuItem>(new NotFoundError($"the requested menu item '{id}' is not found."));
        }

        return menuItem;
    }

    public async Task<Result<MenuItem>> GetMenuItemByNameAsync(string name)
    {
        QueryOptions<MenuItem> options = new QueryOptions<MenuItem>
        {
            Where = g => g.Name == name
        };

        MenuItem? menuItem = await _menuItemRepository.GetAsync(options);
        if (menuItem == null)
        {
            return Result.Fail<MenuItem>(new NotFoundError($"the requested menu item '{name}' is not found."));
        }

        return menuItem;
    }

    public async Task<Result<MenuItem>> GetMenuItemWithRestaurantGenreByNameAsync(string name)
    {
        QueryOptions<MenuItem> options = new QueryOptions<MenuItem>
        {
            SetIncludes = "Restaurant, Genre",
            Where = g => g.Name == name
        };

        MenuItem? menuItem = await _menuItemRepository.GetAsync(options);
        if (menuItem == null)
        {
            return Result.Fail<MenuItem>(new NotFoundError($"the requested menu item '{name}' is not found."));
        }

        return menuItem;
    }

    public async Task<int> GetTotalMenuItemsAsync()
    {
        return await _menuItemRepository.GetCountAsync();
    }

    public async Task<Result<Guid>> CreateMenuItemAsync(MenuItem menuItem)
    {
        MenuItem? isMenuItemExist = await _menuItemRepository.GetAsync(menuItem.MenuItemId);
        if (isMenuItemExist != null)
        {
            return Result.Fail<Guid>(
                new DuplicateError($"the requested menu item '{menuItem.Name}' is already exists."));
        }

        MenuItem createdMenuItem = await _menuItemRepository.AddAsync(menuItem);

        return createdMenuItem.MenuItemId;
    }

    public async Task<Result> UpdateMenuItemAsync(Guid id, MenuItem menuItem)
    {
        MenuItem? isMenuItemExist = await _menuItemRepository.GetAsync(id);
        if (isMenuItemExist == null)
        {
            return Result.Fail(
                new NotFoundError($"the requested menu item '{menuItem.Name}' is not found."));
        }

        await _menuItemRepository.UpdateMenuItem(menuItem);

        return Result.Ok();
    }

    public async Task<Result> DeleteMenuItemAsync(Guid id)
    {
        MenuItem? menuItem = await _menuItemRepository.GetAsync(id);
        if (menuItem == null)
        {
            return Result.Fail(new NotFoundError($"the requested menu item '{id}' is not found."));
        }

        await _menuItemRepository.DeleteAsync(menuItem);

        return Result.Ok();
    }
}