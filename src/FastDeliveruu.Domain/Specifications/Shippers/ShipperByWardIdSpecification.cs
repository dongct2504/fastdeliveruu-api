using FastDeliveruu.Domain.Entities.Identity;

namespace FastDeliveruu.Domain.Specifications.Shippers;

public class ShipperByWardIdSpecification : Specification<Shipper>
{
    public ShipperByWardIdSpecification(int wardId)
        : base(s => s.WardId == wardId)
    {
    }
}
