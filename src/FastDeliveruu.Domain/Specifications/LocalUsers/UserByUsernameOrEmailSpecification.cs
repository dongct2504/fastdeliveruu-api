using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.LocalUsers;

public class UserByUsernameOrEmailSpecification : Specification<LocalUser>
{
    public UserByUsernameOrEmailSpecification(string username, string email)
        : base(lc => lc.UserName == username || lc.Email == email)
    {
    }
}
