using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Interfaces;

public interface IWishListRepository : IRepository<WishList>
{
    void Update(WishList wishList);
}
