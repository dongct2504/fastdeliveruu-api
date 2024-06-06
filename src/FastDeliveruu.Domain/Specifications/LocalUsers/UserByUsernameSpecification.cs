using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.LocalUsers;

public class UserByUsernameSpecification : Specification<LocalUser>
{
    public UserByUsernameSpecification(string username)
        : base(lc => lc.UserName == username)
    {
    }
}
