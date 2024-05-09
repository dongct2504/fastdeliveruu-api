using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Infrastructure.Data;

namespace FastDeliveruu.Infrastructure.Repositories;

public class LocalUserRepository : Repository<LocalUser>, ILocalUserRepository
{
    public LocalUserRepository(Data.FastDeliveruuDbContext context) : base(context)
    {
    }

    public async Task UpdateAsync(LocalUser localUser)
    {
        _dbContext.Update(localUser);
        await _dbContext.SaveChangesAsync();
    }
}