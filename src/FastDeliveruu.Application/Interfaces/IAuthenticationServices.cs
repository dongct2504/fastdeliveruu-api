using FastDeliveruu.Application.Dtos.LocalUserDtos;

namespace FastDeliveruu.Application.Interfaces;

public interface IAuthenticationServices
{
    Task<AuthenticationResult> RegisterAsync(RegisterationRequestDto registerationRequestDto);

    Task<AuthenticationResult> LoginAsync(LoginRequestDto loginRequestDto);
}