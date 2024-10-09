using FastDeliveruu.Domain.Entities.Identity;

namespace FastDeliveruu.Application.Interfaces;

public interface IJwtTokenGenerator
{
    Task<string> GenerateTokenForUserAsync(AppUser user);
    string GenerateTokenForShipper(Shipper shipper);
}