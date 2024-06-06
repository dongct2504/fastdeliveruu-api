using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.Shippers;

public class ShipperByEmailSpecification : Specification<Shipper>
{
    public ShipperByEmailSpecification(string email)
        : base(s => s.Email == email)
    {
    }
}
