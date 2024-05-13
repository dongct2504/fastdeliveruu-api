using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Infrastructure.Repositories;

public class GenreRepository : Repository<Genre>, IGenreRepository
{
    public GenreRepository(Data.FastDeliveruuDbContext context) : base(context)
    {
    }

    public async Task UpdateAsync(Genre genre)
    {
        _dbContext.Update(genre);
        await _dbContext.SaveChangesAsync();
    }
}