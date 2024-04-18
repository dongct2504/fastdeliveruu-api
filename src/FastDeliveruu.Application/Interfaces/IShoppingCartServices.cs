using FastDeliveruu.Domain.Entities;
using FluentResults;

namespace FastDeliveruu.Application.Interfaces;

public interface IShoppingCartServices
{
    Task<IEnumerable<ShoppingCart>> GetAllShoppingCartsAsync();
    Task<IEnumerable<ShoppingCart>> GetAllShoppingCartsByUserIdAsync(Guid userId);

    Task<Result<ShoppingCart>> GetShoppingCartByUserIdMenuItemIdAsync(Guid userId, Guid menuItemId);

    Task<int> GetTotalShoppingCartsAsync();
    Task<int> GetTotalShoppingCartsByUserIdAsync(Guid userId);

    Task<Result> AddToShoppingCartAsync(ShoppingCart shoppingCart);
    Task<Result> UpdateShoppingCartAsync(Guid menuItemId, ShoppingCart shoppingCart);
    Task<Result> DeleteShoppingCartAsync(Guid userId, Guid menuItemId);
}