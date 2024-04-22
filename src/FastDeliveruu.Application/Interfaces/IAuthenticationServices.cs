using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Dtos.ShipperDtos;
using FluentResults;

namespace FastDeliveruu.Application.Interfaces;

public interface IAuthenticationServices
{
    Task<Result<AuthenticationResult>> RegisterAsync(RegisterationRequestDto request);
    Task<Result<AuthenticationShipperResult>> RegisterShipperAsync(RegisterationShipperDto request);

    Task<Result<AuthenticationResult>> LoginAsync(LoginRequestDto request);
    Task<Result<AuthenticationShipperResult>> LoginShipperAsync(LoginShipperDto request);

    Task<Result<bool>> IsEmailConfirm(string token, string email);
}