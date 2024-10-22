using FastDeliveruu.Domain.Entities.Identity;

namespace FastDeliveruu.Domain.Specifications.Shippers;

public class ShipperByCityIdSpecification : Specification<Shipper>
{
    public ShipperByCityIdSpecification(int cityId)
        : base(s => s.CityId == cityId)
    {
    }
}
