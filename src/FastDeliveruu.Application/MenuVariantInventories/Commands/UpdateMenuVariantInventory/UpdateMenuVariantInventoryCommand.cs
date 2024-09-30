using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.MenuVariantInventories.Commands.UpdateMenuVariantInventory;

public class UpdateMenuVariantInventoryCommand : IRequest<Result<MenuVariantInventoryDto>>
{
    public Guid Id { get; set; }

    public Guid MenuVariantId { get; set; }

    public int QuantityAvailable { get; set; }

    public int QuantityReserved { get; set; }
}
