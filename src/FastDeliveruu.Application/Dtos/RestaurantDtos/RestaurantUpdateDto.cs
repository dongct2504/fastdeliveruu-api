using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FastDeliveruu.Application.Dtos.RestaurantDtos;

public class RestaurantUpdateDto
{
    [Required(ErrorMessage = "Vui lòng nhập tên cửa hàng.")]
    [MaxLength(126, ErrorMessage = "Độ dài phải bé hơn 126.")]
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    public string PhoneNumber { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập đã xác nhận hay chưa.")]
    public bool IsVerify { get; set; }

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

    public IFormFile? ImageFile { get; set; }
}