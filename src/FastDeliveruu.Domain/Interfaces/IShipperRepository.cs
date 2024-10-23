using FastDeliveruu.Domain.Entities.Identity;

namespace FastDeliveruu.Domain.Interfaces;

public interface IShipperRepository : IRepository<Shipper>
{
    Task<Shipper?> FindNearestShipper(decimal latitude, decimal longitude);

    void Update(Shipper shipper);
}
