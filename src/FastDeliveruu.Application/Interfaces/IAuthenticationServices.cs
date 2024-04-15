using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FluentResults;

namespace FastDeliveruu.Application.Interfaces;

public interface IAuthenticationServices
{
    Task<Result<AuthenticationResult>> RegisterAsync(RegisterationRequestDto registerationRequestDto);

    Task<Result<AuthenticationResult>> LoginAsync(LoginRequestDto loginRequestDto);

    Task<Result<bool>> IsEmailConfirm(string token, string email);
}