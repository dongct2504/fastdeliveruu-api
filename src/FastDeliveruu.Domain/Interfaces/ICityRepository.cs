using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Interfaces;

public interface ICityRepository : IRepository<City>
{
    void Update(City city);
}
