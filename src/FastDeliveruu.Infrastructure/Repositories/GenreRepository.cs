using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Infrastructure.Data;

namespace FastDeliveruu.Infrastructure.Repositories;

public class GenreRepository : Repository<Genre>, IGenreRepository
{
    public GenreRepository(FastdeliveruuContext context) : base(context)
    {
    }

    public async Task UpdateAsync(Genre genre)
    {
        _dbContext.Update(genre);
        await _dbContext.SaveChangesAsync();
    }
}