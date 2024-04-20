using FastDeliveruu.Domain.Entities;
using FluentResults;

namespace FastDeliveruu.Application.Interfaces;

public interface IShoppingCartServices
{
    Task<IEnumerable<ShoppingCart>> GetAllShoppingCartsAsync();
    Task<IEnumerable<ShoppingCart>> GetAllShoppingCartsByUserIdAsync(Guid userId);

    Task<Result<ShoppingCart>> GetShoppingCartByUserIdMenuItemIdAsync(Guid userId, long menuItemId);

    Task<int> GetTotalShoppingCartsAsync();
    Task<int> GetTotalShoppingCartsByUserIdAsync(Guid userId);

    Task<Result> AddToShoppingCartAsync(ShoppingCart shoppingCart);
    Task<Result> UpdateShoppingCartAsync(long menuItemId, ShoppingCart shoppingCart);
    Task<Result> DeleteShoppingCartAsync(Guid userId, long menuItemId);
}