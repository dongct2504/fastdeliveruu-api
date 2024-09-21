using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Infrastructure.Repositories;

public class GenreRepository : Repository<Genre>, IGenreRepository
{
    public GenreRepository(FastDeliveruuDbContext context) : base(context)
    {
    }

    public void Update(Genre genre)
    {
        _dbContext.Update(genre);
    }
}