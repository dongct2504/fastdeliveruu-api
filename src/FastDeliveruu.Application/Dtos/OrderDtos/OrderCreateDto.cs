using System.ComponentModel.DataAnnotations;
using FastDeliveruu.Application.Dtos.ShoppingCartDtos;

namespace FastDeliveruu.Application.Dtos.OrderDtos;

public class OrderCreateDto
{
    [Required(ErrorMessage = "Vui lòng nhập tên người nhận.")]
    [MaxLength(128, ErrorMessage = "Tên người nhận không dài quá 128 kí tự.")]
    public string ReceiverName { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    public string PhoneNumber { get; set; } = null!;

    [Required]
    public decimal TotalAmount { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ đường.")]
    public string Address { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập phường.")]
    public string Ward { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập quận.")]
    public string District { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập thành phố.")]
    public string City { get; set; } = null!;
}