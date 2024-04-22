namespace FastDeliveruu.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid id, string email, string userName, string role);
    string GenerateEmailConfirmationToken(Guid id, string email);
}