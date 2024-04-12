using System.Data;
using Dapper;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Application.Services;

public class ShoppingCartServices : IShoppingCartServices
{
    private readonly ISP_Call _sP_Call;

    public ShoppingCartServices(ISP_Call sP_Call)
    {
        _sP_Call = sP_Call;
    }

    public async Task<IEnumerable<ShoppingCart>> GetAllShoppingCartsAsync()
    {
        string procedureName = "spGetAllShoppingCarts";

        return await _sP_Call.ListAsync<ShoppingCart>(procedureName);
    }

    public async Task<IEnumerable<ShoppingCart>> GetAllShoppingCartsByUserIdAsync(int userId)
    {
        string procedureName = "spGetAllShoppingCartsByUserId";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@LocalUserId", userId);

        return await _sP_Call.ListAsync<ShoppingCart>(procedureName, parameters);
    }

    public async Task<ShoppingCart?> GetShoppingCartByIdAsync(int id)
    {
        string procedureName = "spGetShoppingCartById";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ShoppingCartId", id);

        return await _sP_Call.OneRecordAsync<ShoppingCart>(procedureName, parameters);
    }

    public async Task<ShoppingCart?> GetShoppingCartByUserIdMenuItemIdAsync(int userId, int menuItemId)
    {
        string procedureName = "spGetShoppingCartByUserIdMenuItemId";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@LocalUserId", userId);
        parameters.Add("@MenuItemId", menuItemId);

        return await _sP_Call.OneRecordAsync<ShoppingCart>(procedureName, parameters);
    }

    public async Task<int> CreateShoppingCartAsync(ShoppingCart shoppingCart)
    {
        string procedureName = "spCreateShoppingCart";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@MenuItemId", shoppingCart.MenuItemId);
        parameters.Add("@LocalUserId", shoppingCart.LocalUserId);
        parameters.Add("@Quantity", shoppingCart.Quantity);
        parameters.Add("@CreatedAt", shoppingCart.CreatedAt);
        parameters.Add("@UpdatedAt", shoppingCart.UpdatedAt);
        parameters.Add("@ShoppingCartId", DbType.Int32, direction: ParameterDirection.Output);

        await _sP_Call.ExecuteAsync(procedureName, parameters);

        return parameters.Get<int>("@ShoppingCartId");
    }

    public async Task UpdateShoppingCartAsync(ShoppingCart shoppingCart)
    {
        string procedureName = "spUpdateShoppingCart";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@MenuItemId", shoppingCart.MenuItemId);
        parameters.Add("@LocalUserId", shoppingCart.LocalUserId);
        parameters.Add("@Quantity", shoppingCart.Quantity);
        parameters.Add("@UpdatedAt", shoppingCart.UpdatedAt);

        await _sP_Call.ExecuteAsync(procedureName, parameters);
    }

    public async Task DeleteShoppingCartAsync(int id)
    {
        string procedureName = "spDeleteShoppingCart";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ShoppingCartId", id);

        await _sP_Call.ExecuteAsync(procedureName, parameters);
    }
}