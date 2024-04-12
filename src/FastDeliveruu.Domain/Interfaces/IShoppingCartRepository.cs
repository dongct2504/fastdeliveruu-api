using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Interfaces;

public interface IShoppingCartRepository : IRepository<ShoppingCart>
{
    Task UpdateShoppingCart(ShoppingCart shoppingCart);
}