using FastDeliveruu.Application.Dtos.MenuVariantDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.MenuVariants.Commands.CreateMenuVariant;

public class CreateMenuVariantCommand : IRequest<Result<MenuVariantDto>>
{
    public Guid MenuItemId { get; set; }

    public string VarietyName { get; set; } = null!;

    public decimal Price { get; set; }

    public decimal DiscountPercent { get; set; }
}
