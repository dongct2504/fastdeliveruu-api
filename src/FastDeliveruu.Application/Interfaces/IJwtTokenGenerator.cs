using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(LocalUser localUser);
    string GenerateEmailConfirmationToken(LocalUser localUser);
}