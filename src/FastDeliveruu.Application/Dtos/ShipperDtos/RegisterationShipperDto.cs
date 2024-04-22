using System.ComponentModel.DataAnnotations;

namespace FastDeliveruu.Application.Dtos.ShipperDtos;

public class RegisterationShipperDto
{
    [Required(ErrorMessage = "Vui lòng nhập tên.")]
    [MaxLength(50, ErrorMessage = "Vui lòng nhập tên ít hơn 50 kí tự.")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập họ.")]
    [MaxLength(50, ErrorMessage = "Vui lòng nhập họ ít hơn 50 kí tự.")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập ngày sinh.")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
    [DataType(DataType.Date, ErrorMessage = "date is not a corret format")]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập username.")]
    [MaxLength(128, ErrorMessage = "Vui lòng nhập username ít hơn 128 kí tự.")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập căn cước công dân.")]
    [MaxLength(12, ErrorMessage = "Vui lòng nhập username ít hơn 12 kí tự.")]
    public string Cccd { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập giấy phép lái xe.")]
    [MaxLength(12, ErrorMessage = "Vui lòng nhập username ít hơn 12 kí tự.")]
    public string DriverLicense { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [MaxLength(15, ErrorMessage = "Vui lòng nhập số điện thoại ít hơn 15 kí tự.")]
    [Phone(ErrorMessage = "Vui lòng nhập số điện thoại hợp lệ.")]
    public string PhoneNumber { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Vui lòng nhập email hợp lệ.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
    [MaxLength(128, ErrorMessage = "Độ dài phải bé hơn 128.")]
    public string Address { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập phường.")]
    [MaxLength(50, ErrorMessage = "Độ dài phải bé hơn 50.")]
    public string Ward { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập quận.")]
    [MaxLength(30, ErrorMessage = "Độ dài phải bé hơn 30.")]
    public string District { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập thành phố.")]
    [MaxLength(30, ErrorMessage = "Độ dài phải bé hơn 30.")]
    public string City { get; set; } = null!;

    public string? VehicleType { get; set; }
}
