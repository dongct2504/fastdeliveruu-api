using System.ComponentModel.DataAnnotations;

namespace FastDeliveruu.Application.Dtos.LocalUserDtos;

public class RegisterationRequestDto
{
    [Required(ErrorMessage = "Vui lòng nhập tên.")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập họ.")]
    public string LastName { get; set; } = null!;

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

    public string? Role { get; set; }
}