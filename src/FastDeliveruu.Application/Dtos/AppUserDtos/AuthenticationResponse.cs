namespace FastDeliveruu.Application.Dtos.AppUserDtos;

public class AuthenticationResponse
{
    public AppUserDto AppUserDto { get; set; } = null!;

    public string Token { get; set; } = null!;
}
