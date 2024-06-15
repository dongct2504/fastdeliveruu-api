using FastDeliveruu.Application.Dtos.AppUserDtos;

namespace FastDeliveruu.Application.Dtos.LocalUserDtos;

public class AuthenticationResponse
{
    public AppUserDto AppUserDto { get; set; } = null!;

    public string Token { get; set; } = null!;
}