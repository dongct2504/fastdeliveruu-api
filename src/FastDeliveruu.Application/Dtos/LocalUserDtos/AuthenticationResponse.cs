namespace FastDeliveruu.Application.Dtos.LocalUserDtos;

public class AuthenticationResponse
{
    public LocalUserDto LocalUserDto { get; set; } = null!;

    public string Token { get; set; } = null!;
}