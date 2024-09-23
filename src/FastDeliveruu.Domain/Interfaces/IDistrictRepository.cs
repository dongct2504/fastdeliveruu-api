using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Interfaces;

public interface IDistrictRepository : IRepository<District>
{
    void Update(District district);
}
