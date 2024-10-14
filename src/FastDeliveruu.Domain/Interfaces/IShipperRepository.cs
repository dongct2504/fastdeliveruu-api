using FastDeliveruu.Domain.Entities.Identity;

namespace FastDeliveruu.Domain.Interfaces;

public interface IShipperRepository : IRepository<Shipper>
{
    void Update(Shipper shipper);
}
