using System.ComponentModel.DataAnnotations;

namespace FastDeliveruu.Application.Dtos.LocalUserDtos;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Vui lòng nhập user name.")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    public string Password { get; set; } = null!;
}