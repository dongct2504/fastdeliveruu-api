using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.Shippers;

public class ShipperByUsernameSpecification : Specification<Shipper>
{
    public ShipperByUsernameSpecification(string username)
        : base(s => s.UserName == username)
    {
    }
}
