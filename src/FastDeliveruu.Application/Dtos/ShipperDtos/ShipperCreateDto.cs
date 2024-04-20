using System.ComponentModel.DataAnnotations;

namespace FastDeliveruu.Application.Dtos.ShipperDtos;

public class ShipperCreateDto
{
    [Required(ErrorMessage = "Vui lòng nhập tên.")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập họ.")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [Phone(ErrorMessage = "Vui lòng nhập số điện thoại hợp lệ.")]
    public string PhoneNumber { get; set; } = null!;

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
