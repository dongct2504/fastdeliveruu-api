using FastDeliveruu.Domain.Entities.Identity;

namespace FastDeliveruu.Domain.Specifications.Shippers;

public class ShipperByDistrictIdSpecification : Specification<Shipper>
{
    public ShipperByDistrictIdSpecification(int districtId)
        : base(s => s.DistrictId == districtId)
    {
    }
}
