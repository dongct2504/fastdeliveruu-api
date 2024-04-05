using FastDeliveruu.Application.Dtos.MenuItemDtos;

namespace FastDeliveruu.Api.Dtos;

public class MenuItemUpdateWithImageDto
{
    public MenuItemUpdateDto MenuItemUpdateDto { get; set; } = null!;
    public IFormFile? ImageFile { get; set; }
}