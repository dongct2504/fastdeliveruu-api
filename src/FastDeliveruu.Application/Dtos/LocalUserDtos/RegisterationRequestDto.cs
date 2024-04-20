using System.ComponentModel.DataAnnotations;

namespace FastDeliveruu.Application.Dtos.LocalUserDtos;

public class RegisterationRequestDto
{
    [Required(ErrorMessage = "Vui lòng nhập username.")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [Phone(ErrorMessage = "Vui lòng nhập số điện thoại hợp lệ.")]
    public string PhoneNumber { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Vui lòng nhập email hợp lệ.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    public string Password { get; set; } = null!;

    public string? Address { get; set; }

    public string? Ward { get; set; }

    public string? District { get; set; }

    public string? City { get; set; }

    public string? Role { get; set; }
}