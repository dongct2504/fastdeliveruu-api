using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Entities.Identity;
using FastDeliveruu.Domain.Identity.CustomManagers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Infrastructure.SeedData.Seeders;

public class UserRoleSeeder : IDataSeeder
{
    private readonly RoleManager<AppRole> _roleManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly ShipperManager _shipperManager;

    public UserRoleSeeder(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager, ShipperManager shipperManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _shipperManager = shipperManager;
    }

    public int Order => 2;

    public async Task SeedAsync(FastDeliveruuDbContext context)
    {
        await SeedRolesAsync();
        await SeedUsersAsync();
        await SeedShippersAsync(context);
    }

    private async Task SeedRolesAsync()
    {
        string[] roleNames = { RoleConstants.Customer, RoleConstants.Staff, RoleConstants.Shipper, RoleConstants.Admin };
        foreach (var roleName in roleNames)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                //await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                await _roleManager.CreateAsync(new AppRole(roleName));
            }
        }
    }

    private async Task SeedUsersAsync()
    {
        var usersToSeed = new List<(string UserName, string Role)>
        {
            ("admin1", RoleConstants.Admin),
            ("admin2", RoleConstants.Admin),
            ("staff1", RoleConstants.Staff),
            ("staff2", RoleConstants.Staff),
            ("staff3", RoleConstants.Staff),
            ("customer1", RoleConstants.Customer),
            ("customer2", RoleConstants.Customer),
            ("customer3", RoleConstants.Customer),
            ("customer4", RoleConstants.Customer),
        };

        foreach (var (userName, role) in usersToSeed)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = userName,
                    Email = $"{userName}@example.com",
                    EmailConfirmed = true,
                    PhoneNumber = "+841234567890",
                    PhoneNumberConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, "Pa$$w0rd");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
                else
                {
                    throw new Exception($"Seeding user {userName} failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                if (!await _userManager.IsInRoleAsync(user, role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }

    private async Task SeedShippersAsync(FastDeliveruuDbContext context)
    {
        var now = DateTime.UtcNow;

        // load address đã seed
        var hcm = await context.Cities
            .Include(c => c.Districts).ThenInclude(d => d.Wards)
            .FirstOrDefaultAsync(c => c.Name == "Hồ Chí Minh");

        var hanoi = await context.Cities
            .Include(c => c.Districts).ThenInclude(d => d.Wards)
            .FirstOrDefaultAsync(c => c.Name == "Hà Nội");

        if (hcm == null || hanoi == null) return; // chưa seed AddressSeeder

        var shippersData = new List<(string UserName, string Email, string House, string Street, City City, District District, Ward Ward)>
        {
            ("shipper1", "shipper1@example.com", "12", "Lê Lợi", hcm,
                hcm.Districts.First(d => d.Name == "Quận 1"),
                hcm.Districts.First(d => d.Name == "Quận 1").Wards.First()),

            ("shipper2", "shipper2@example.com", "34", "Hàng Bài", hanoi,
                hanoi.Districts.First(d => d.Name == "Hoàn Kiếm"),
                hanoi.Districts.First(d => d.Name == "Hoàn Kiếm").Wards.First()),

            ("shipper3", "shipper3@example.com", "45", "Nguyễn Thị Minh Khai", hcm,
                hcm.Districts.First(d => d.Name == "Quận 3"),
                hcm.Districts.First(d => d.Name == "Quận 3").Wards.First())
        };

        foreach (var (userName, email, house, street, city, district, ward) in shippersData)
        {
            var user = await _shipperManager.FindByNameAsync(userName);
            if (user == null)
            {
                user = new Shipper
                {
                    UserName = userName,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = userName,
                    LastName = userName,
                    CitizenIdentification = "123456789",
                    PhoneNumber = "+841234567890",
                    PhoneNumberConfirmed = true,
                    HouseNumber = house,
                    StreetName = street,
                    CityId = city.Id,
                    DistrictId = district.Id,
                    WardId = ward.Id,
                    CreatedAt = now
                };

                var result = await _shipperManager.CreateAsync(user, "Pa$$w0rd");
                if (!result.Succeeded)
                {
                    throw new Exception(
                        $"Seeding shipper {userName} failed: {string.Join(", ", result.Errors.Select(e => e.Description))}"
                    );
                }
            }
        }
    }
}
