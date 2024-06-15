using FastDeliveruu.Domain.Entities.Identity;

namespace FastDeliveruu.Application.Interfaces;

public interface IJwtTokenGenerator
{
    Task<string> GenerateTokenAsync(AppUser user);
}