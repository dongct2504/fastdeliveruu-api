using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Interfaces;

public interface IWardRepository : IRepository<Ward>
{
    void Update(Ward ward);
}
