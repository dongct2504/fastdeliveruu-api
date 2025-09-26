using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.MenuItemInventories.Commands.CreateMenuItemInventory;

public class UpdateMenuItemInventoryCommand : IRequest<Result<MenuItemInventoryDto>>
{
    public Guid? Id { get; set; }

    public Guid MenuItemId { get; set; }

    public int QuantityAvailable { get; set; }

    public int QuantityReserved { get; set; }
}
