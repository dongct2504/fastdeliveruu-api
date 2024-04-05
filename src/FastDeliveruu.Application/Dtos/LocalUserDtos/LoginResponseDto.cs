namespace FastDeliveruu.Application.Dtos.LocalUserDtos;

public class LoginResponseDto
{
    public LocalUserDto? LocalUserDto { get; set; }

    public string Token { get; set; } = null!;
}