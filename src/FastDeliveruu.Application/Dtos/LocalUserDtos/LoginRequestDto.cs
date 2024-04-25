namespace FastDeliveruu.Application.Dtos.LocalUserDtos;

public class LoginRequestDto
{
    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;
}