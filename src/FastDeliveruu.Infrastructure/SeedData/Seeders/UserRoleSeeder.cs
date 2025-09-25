using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities.Identity;
using FastDeliveruu.Domain.Identity.CustomManagers;
using Microsoft.AspNetCore.Identity;

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
        //await SeedShippersAsync();
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
                    EmailConfirmed = true
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

    private async Task SeedShippersAsync()
    {
        var shippersToSeed = new List<(string UserName, string Role)>
        {
            ("shipper1", RoleConstants.Shipper),
            ("shipper1", RoleConstants.Shipper),
            ("shipper1", RoleConstants.Shipper),
        };

        foreach (var (userName, role) in shippersToSeed)
        {
            var user = await _shipperManager.FindByNameAsync(userName);
            if (user == null)
            {
                user = new Shipper
                {
                    UserName = userName,
                    Email = $"{userName}@example.com",
                    EmailConfirmed = true,
                    FirstName = userName,
                    LastName = userName,
                    CitizenIdentification = "123",
                    HouseNumber = "12",
                    StreetName = "123",
                    WardId = 1,
                    DistrictId = 1,
                    CityId = 1
                };

                var result = await _shipperManager.CreateAsync(user, "Pa$$w0rd");
                if (result.Succeeded)
                {
                    await _shipperManager.AddToRoleAsync(user, role);
                }
                else
                {
                    throw new Exception($"Seeding user {userName} failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                if (!await _shipperManager.IsInRoleAsync(user, role))
                {
                    await _shipperManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}
