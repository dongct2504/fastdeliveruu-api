using FastDeliveruu.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Infrastructure.Identity;

public class FastDeliveruuIdentityDbContext : IdentityDbContext<AppUser>
{
    public FastDeliveruuIdentityDbContext(DbContextOptions<FastDeliveruuIdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
