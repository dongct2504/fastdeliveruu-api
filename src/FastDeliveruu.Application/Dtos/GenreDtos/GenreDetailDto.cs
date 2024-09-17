using FastDeliveruu.Application.Dtos.MenuItemDtos;

namespace FastDeliveruu.Application.Dtos.GenreDtos;

public class GenreDetailDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public IEnumerable<MenuItemDto>? MenuItemDtos { get; set; }
}