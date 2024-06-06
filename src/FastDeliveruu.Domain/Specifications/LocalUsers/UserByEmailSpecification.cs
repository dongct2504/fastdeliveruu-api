using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.LocalUsers;

public class UserByEmailSpecification : Specification<LocalUser>
{
    public UserByEmailSpecification(string email)
        : base(lc => lc.Email == email)
    {
    }
}
