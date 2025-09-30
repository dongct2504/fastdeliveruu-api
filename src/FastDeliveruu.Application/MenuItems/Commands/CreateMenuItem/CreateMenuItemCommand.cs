using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FastDeliveruu.Application.MenuItems.Commands.CreateMenuItem;

public class CreateMenuItemCommand : IRequest<Result<MenuItemDto>>
{
    public Guid RestaurantId { get; set; }
    public Guid GenreId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public decimal DiscountPercent { get; set; }

    public int QuantityAvailable { get; set; }
    public int QuantityReserved { get; set; }

    public IFormFile ImageFile { get; set; } = null!;
}
