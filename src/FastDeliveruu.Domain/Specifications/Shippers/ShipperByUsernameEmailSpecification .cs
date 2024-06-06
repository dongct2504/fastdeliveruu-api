using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.Shippers;

public class ShipperByUsernameOrEmailSpecification : Specification<Shipper>
{
    public ShipperByUsernameOrEmailSpecification(string username, string email)
        : base(s => s.UserName == username || s.Email == email)
    {
    }
}
