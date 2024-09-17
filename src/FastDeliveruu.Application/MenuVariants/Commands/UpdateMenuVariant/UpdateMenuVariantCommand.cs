using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.MenuVariants.Commands.UpdateMenuVariant;

public class UpdateMenuVariantCommand : IRequest<Result>
{
    public Guid Id { get; set; }

    public Guid MenuItemId { get; set; }

    public string VarietyName { get; set; } = null!;

    public decimal Price { get; set; }

    public decimal DiscountPercent { get; set; }
}
