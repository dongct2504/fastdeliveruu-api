using System.ComponentModel.DataAnnotations;

namespace FastDeliveruu.Application.Dtos.ShoppingCartDtos;

public class ShoppingCartUpdateDto
{
    [Required(ErrorMessage = "Vui lòng chọn số lượng.")]
    public int Quantity { get; set; }
}