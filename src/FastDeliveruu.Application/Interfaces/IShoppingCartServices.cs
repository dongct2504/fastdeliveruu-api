using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Application.Interfaces;

public interface IShoppingCartServices
{
    Task<IEnumerable<ShoppingCart>> GetAllShoppingCartsAsync();
    Task<IEnumerable<ShoppingCart>> GetAllShoppingCartsByUserIdAsync(int userId, int page);

    Task<ShoppingCart?> GetShoppingCartByIdAsync(int id);
    Task<ShoppingCart?> GetShoppingCartByUserIdMenuItemIdAsync(int userId, int menuItemId);

    Task<int> GetTotalShoppingCartsAsync();
    Task<int> GetTotalShoppingCartsByUserIdAsync(int userId);

    Task<int> CreateShoppingCartAsync(ShoppingCart shoppingCart);
    Task UpdateShoppingCartAsync(ShoppingCart shoppingCart);
    Task DeleteShoppingCartAsync(int id);
}