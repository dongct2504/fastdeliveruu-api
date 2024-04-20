using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Interfaces;

public interface IShipperRepository : IRepository<Shipper>
{
    Task UpdateAsync(Shipper shipper);
}
