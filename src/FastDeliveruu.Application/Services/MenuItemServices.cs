using System.Data;
using Dapper;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Application.Dtos;

public class MenuItemServices : IMenuItemServices
{
    private readonly ISP_Call _sP_Call;

    public MenuItemServices(ISP_Call sP_Call)
    {
        _sP_Call = sP_Call;
    }

    public async Task<IEnumerable<MenuItem>> GetAllMenuItemsAsync(int page)
    {
        string procedureName = "spGetAllMenuItemsPaging";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@RowOffSet", PagingConstants.PageSize * (page - 1));
        parameters.Add("@FetchNextRow", PagingConstants.PageSize);

        return await _sP_Call.ListAsync<MenuItem>(procedureName, parameters);
    }

    public async Task<IEnumerable<MenuItemWithRestaurantGenreDto>> GetAllMenuItemsWithRestaurantGenreAsync(
        int page)
    {
        string procedureName = "spGetAllMenuItemsWithRestaurantGenrePaging";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@RowOffSet", PagingConstants.PageSize * (page - 1));
        parameters.Add("@FetchNextRow", PagingConstants.PageSize);

        return await _sP_Call.ListAsync<MenuItemWithRestaurantGenreDto>(procedureName, parameters);
    }

    public async Task<IEnumerable<MenuItemWithRestaurantGenreDto>> GetFilterMenuItemsWithRestaurantGenreAsync(
        int? genreId, int? restaurantId, int page)
    {
        IEnumerable<MenuItemWithRestaurantGenreDto> menuItemDtos = await 
            GetAllMenuItemsWithRestaurantGenreAsync(page);

        if (genreId != null)
        {
            menuItemDtos = menuItemDtos.Where(mi => mi.GenreId == genreId);
        }

        if (restaurantId != null)
        {
            menuItemDtos = menuItemDtos.Where(mi => mi.RestaurantId == restaurantId);
        }

        return menuItemDtos;
    }

    public async Task<MenuItem?> GetMenuItemByIdAsync(int id)
    {
        string procedureName = "spGetMenuItemById";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@MenuItemId", id);

        return await _sP_Call.OneRecordAsync<MenuItem>(procedureName, parameters);
    }

    public async Task<MenuItemWithRestaurantGenreDto?> GetMenuItemWithRestaurantGenreByIdAsync(int id)
    {
        string procedureName = "spGetMenuItemWithRestaurantGenreById";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@MenuItemId", id);

        return await _sP_Call.OneRecordAsync<MenuItemWithRestaurantGenreDto>(procedureName, parameters);
    }

    public async Task<MenuItem?> GetMenuItemByNameAsync(string name)
    {
        string procedureName = "spGetMenuItemWithRestaurantGenreByName";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Name", name);

        return await _sP_Call.OneRecordAsync<MenuItem>(procedureName, parameters);
    }

    public async Task<MenuItemWithRestaurantGenreDto?> GetMenuItemWithRestaurantGenreByNameAsync(string name)
    {
        string procedureName = "spGetMenuItemsWithRestaurantGenreByName";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Name", name);

        return await _sP_Call.OneRecordAsync<MenuItemWithRestaurantGenreDto>(procedureName);
    }

    public async Task<int> CreateMenuItemAsync(MenuItem menuItem)
    {
        string procedureName = "spCreateMenuItem";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@RestaurantId", menuItem.RestaurantId);
        parameters.Add("@GenreId", menuItem.GenreId);
        parameters.Add("@Name", menuItem.Name);
        parameters.Add("@Description", menuItem.Description);
        parameters.Add("@Inventory", menuItem.Inventory);
        parameters.Add("@Price", menuItem.Price);
        parameters.Add("@DiscountPercent", menuItem.DiscountPercent);
        parameters.Add("@ImageUrl", menuItem.ImageUrl);
        parameters.Add("@CreatedAt", menuItem.CreatedAt);
        parameters.Add("@UpdatedAt", menuItem.UpdatedAt);
        parameters.Add("@MenuItemId", DbType.Int32, direction: ParameterDirection.Output);

        await _sP_Call.ExecuteAsync(procedureName, parameters);

        return parameters.Get<int>("@MenuItemId");
    }

    public async Task UpdateMenuItemAsync(MenuItem menuItem)
    {
        string procedureName = "spUpdateMenuItem";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@MenuItemId", menuItem.MenuItemId);
        parameters.Add("@RestaurantId", menuItem.RestaurantId);
        parameters.Add("@GenreId", menuItem.GenreId);
        parameters.Add("@Name", menuItem.Name);
        parameters.Add("@Description", menuItem.Description);
        parameters.Add("@Inventory", menuItem.Inventory);
        parameters.Add("@Price", menuItem.Price);
        parameters.Add("@DiscountPercent", menuItem.DiscountPercent);
        parameters.Add("@ImageUrl", menuItem.ImageUrl);
        parameters.Add("@UpdatedAt", menuItem.UpdatedAt);

        await _sP_Call.ExecuteAsync(procedureName, parameters);
    }

    public async Task DeleteMenuItemAsync(int id)
    {
        string procedureName = "spDeleteMenuItem";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@MenuItemId", id);

        await _sP_Call.ExecuteAsync(procedureName, parameters);
    }
}