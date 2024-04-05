using System.ComponentModel.DataAnnotations;

namespace FastDeliveruu.Application.Dtos.MenuItemDtos;

public class MenuItemUpdateDto
{
    [Required(ErrorMessage = "Vui lòng chọn cửa hàng.")]
    public int RestaurantId { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn thể loại.")]
    public int GenreId { get; set; }

    [Required(ErrorMessage = "Vui lòng đặt tên cho món.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập mô tả.")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập hàng tồn.")]
    public int Inventory { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập giá.")]
    public decimal Price { get; set; }

    public decimal DiscountPercent { get; set; }
}