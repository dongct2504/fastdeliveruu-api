using Microsoft.AspNetCore.Identity;

namespace FastDeliveruu.Domain.Entities.Identity;

public class AppUserRole : IdentityUserRole<Guid>
{
    public virtual AppUser AppUser { get; set; } = null!;
    public virtual AppRole AppRole { get; set; } = null!;
}
