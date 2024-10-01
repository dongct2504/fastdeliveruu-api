using Microsoft.AspNetCore.Identity;

namespace FastDeliveruu.Domain.Entities.Identity;

public class AppUserRoles : IdentityUserRole<Guid>
{
    public AppUser AppUser { get; set; } = null!;
    public AppRole AppRoles { get; set; } = null!;
}
