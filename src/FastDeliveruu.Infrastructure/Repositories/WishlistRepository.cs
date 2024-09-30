using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Infrastructure.Repositories;

public class WishlistRepository : Repository<WishList>, IWishListRepository
{
    public WishlistRepository(FastDeliveruuDbContext context) : base(context)
    {
    }

    public void Update(WishList wishList)
    {
        _dbContext.Update(wishList);
    }
}
