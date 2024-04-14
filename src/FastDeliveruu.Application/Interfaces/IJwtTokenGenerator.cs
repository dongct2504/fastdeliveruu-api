using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateTokenAsync(LocalUser localUser);
}