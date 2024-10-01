using Microsoft.AspNetCore.Identity;

namespace FastDeliveruu.Domain.Entities.Identity;

public class AppRole : IdentityRole<Guid>
{
    public AppRole()
    {
    }

    public AppRole(string roleName) : base(roleName)
    {
    }

    public ICollection<AppUserRoles> AppUserRoles { get; set; } = new List<AppUserRoles>();
}
