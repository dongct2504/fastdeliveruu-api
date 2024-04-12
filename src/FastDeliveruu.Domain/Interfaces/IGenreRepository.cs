using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Interfaces;

public interface IGenreRepository : IRepository<Genre>
{
    Task UpdateAsync(Genre genre);
}