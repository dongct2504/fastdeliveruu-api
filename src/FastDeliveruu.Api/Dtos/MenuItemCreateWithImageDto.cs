using FastDeliveruu.Application.Dtos.MenuItemDtos;

namespace FastDeliveruu.Api.Dtos;

public class MenuItemCreateWithImageDto
{
    public MenuItemCreateDto MenuItemCreateDto { get; set; } = null!;
    public IFormFile? ImageFile { get; set; }
}