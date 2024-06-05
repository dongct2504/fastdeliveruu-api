using FastDeliveruu.Application.Dtos.MenuItemDtos;
using MediatR;

namespace FastDeliveruu.Application.MenuItems.Queries.SearchMenuItems;

public class SearchMenuItemsQuery : IRequest<IEnumerable<MenuItemDto>>
{
    public SearchMenuItemsQuery(decimal amount, decimal discountPercent)
    {
        Amount = amount;
        DiscountPercent = discountPercent;
    }

    public decimal Amount { get; }

    public decimal DiscountPercent { get; }
}
