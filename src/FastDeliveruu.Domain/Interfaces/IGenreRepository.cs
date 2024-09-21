using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Interfaces;

public interface IGenreRepository : IRepository<Genre>
{
    void Update(Genre genre);
}